namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// The FilterCollection class provides efficient, reusable filtering based on a filter
/// predicate, and associate change events.
/// </summary>
public sealed class FilterCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : INotifyPropertyChanged {
    private readonly List<AddJournalEntry> _addJournal = [];
    private readonly List<T> _cachedFilteredItems = [];
    private readonly Predicate<T> _filter;
    private readonly string _filterPropertyName;
    private readonly List<T> _items = [];
    private readonly object _lock = new();
    private readonly List<int> _removeJournal = [];
    private readonly Comparison<int> _removeJournalSortComparison = (x, y) => Comparer<int>.Default.Compare(y, x);
    private bool _shouldRebuildCache = true;

    /// <summary>
    /// Occurs when [collection changed].
    /// </summary>
    public event EventHandler<CollectionChangedEventArgs<T>>? CollectionChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterCollection{T}" /> class.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="filterPropertyName">Name of the filter property.</param>
    public FilterCollection(
        Predicate<T> filter,
        string filterPropertyName) {
        this._filter = filter;
        this._filterPropertyName = filterPropertyName;
    }

    /// <summary>
    /// Gets the number of elements contained in the
    /// <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public int Count => this._cachedFilteredItems.Count;

    /// <summary>
    /// Gets a value indicating whether the
    /// <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">
    /// The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </param>
    public void Add(T item) {
        lock (this._lock) {
            this._addJournal.Add(new AddJournalEntry(this._addJournal.Count, item));
            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(true, item));
        }
    }

    /// <summary>
    /// Adds the specified item. If the item is not the correct time, it gets ignored.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(object item) {
        if (item is T x) {
            this.Add(x);
        }
    }

    /// <summary>
    /// Adds the items.
    /// </summary>
    /// <param name="items">The items.</param>
    public void AddRange(IReadOnlyCollection<T> items) {
        lock (this._lock) {
            foreach (var item in items) {
                this._addJournal.Add(new AddJournalEntry(this._addJournal.Count, item));
            }

            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(true, items));
        }
    }

    /// <summary>
    /// Adds the items.
    /// </summary>
    /// <param name="items">The items.</param>
    public void AddRange(IEnumerable<object> items) {
        var castedItems = items.OfType<T>().ToList();

        if (castedItems.Any()) {
            this.AddRange(castedItems);
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear() {
        lock (this._lock) {
            foreach (var t in this._items) {
                this.UnsubscribeFromItemEvents(t);
            }

            var items = this._items;
            this._addJournal.Clear();
            this._removeJournal.Clear();
            this._items.Clear();
            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, items));
        }
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// contains a specific value.
    /// </summary>
    /// <param name="item">
    /// The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </param>
    /// <returns>
    /// true if <paramref name="item" /> is found in the
    /// <see
    ///     cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, false.
    /// </returns>
    public bool Contains(T item) => this._items.Contains(item);

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to
    /// an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="T:System.Array" /> that is the destination of the
    /// elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The
    /// <see cref="T:System.Array" /> must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">
    /// The zero-based index in <paramref name="array" /> at which copying begins.
    /// </param>
    public void CopyTo(T[] array, int arrayIndex) {
        this._items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
    /// through the collection.
    /// </returns>
    public IEnumerator<T> GetEnumerator() => this._cachedFilteredItems.GetEnumerator();

    /// <summary>
    /// Rebuilds the cache.
    /// </summary>
    public void RebuildCache() {
        lock (this._lock) {
            if (this._shouldRebuildCache) {
                this.ProcessRemoveJournal();
                this.ProcessAddJournal();

                // Rebuild the cache
                this._cachedFilteredItems.Clear();
                foreach (var item in this._items) {
                    if (this._filter(item)) {
                        this._cachedFilteredItems.Add(item);
                    }
                }

                this._shouldRebuildCache = false;
            }
        }
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the
    /// <see
    ///     cref="T:System.Collections.Generic.ICollection`1" />
    /// .
    /// </summary>
    /// <param name="item">
    /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </param>
    /// <returns>
    /// true if <paramref name="item" /> was successfully removed from the
    /// <see
    ///     cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, false. This method also
    /// returns false if <paramref name="item" /> is not found in the original
    /// <see
    ///     cref="T:System.Collections.Generic.ICollection`1" />
    /// .
    /// </returns>
    public bool Remove(T item) {
        lock (this._lock) {
            if (this._addJournal.Remove(AddJournalEntry.CreateKey(item))) {
                this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, item));
                return true;
            }

            var index = this._items.IndexOf(item);
            if (index >= 0) {
                this.UnsubscribeFromItemEvents(item);
                this._removeJournal.Add(index);
                this.InvalidateCache();
                this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, item));
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Removes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>A value indicating whether or not the item was removed.</returns>
    public bool Remove(object item) {
        if (item is T x) {
            return this.Remove(x);
        }

        return false;
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._items).GetEnumerator();

    private void InvalidateCache() {
        this._shouldRebuildCache = true;
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == this._filterPropertyName) {
            this.InvalidateCache();
        }
    }

    private void ProcessAddJournal() {
        if (this._addJournal.Count == 0) {
            return;
        }

        foreach (var journal in this._addJournal) {
            this._items.Add(journal.Item);
            this.SubscribeToItemEvents(journal.Item);
        }

        this._addJournal.Clear();
    }

    private void ProcessRemoveJournal() {
        if (this._removeJournal.Count == 0) {
            return;
        }

        // Remove items in reverse. (Technically there exist faster ways to bulk-remove from a
        // variable-length array, but List<T> does not provide such a method.)
        this._removeJournal.Sort(this._removeJournalSortComparison);
        foreach (var t in this._removeJournal) {
            this._items.RemoveAt(t);
        }

        this._removeJournal.Clear();
    }

    private void SubscribeToItemEvents(T item) {
        item.PropertyChanged += this.Item_PropertyChanged;
    }

    private void UnsubscribeFromItemEvents(T item) {
        item.PropertyChanged -= this.Item_PropertyChanged;
    }

    /// <summary>
    /// Add journal entry.
    /// </summary>
    private record AddJournalEntry(int Order, T Item) {

        /// <summary>
        /// Creates the key.
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="item">Item.</param>
        public static AddJournalEntry CreateKey(T item) => new(-1, item);

        /// <inheritdoc />
        public override int GetHashCode() => this.Item.GetHashCode();
    }
}
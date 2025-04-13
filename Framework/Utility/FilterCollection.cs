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
public class FilterCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : INotifyPropertyChanged {
    private readonly List<T> _cachedFilteredItems = [];
    private readonly Predicate<T> _filter;
    private readonly string _filterPropertyName;
    private readonly object _lock = new();
    private readonly Comparison<int> _removeJournalSortComparison = (x, y) => Comparer<int>.Default.Compare(y, x);

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
    /// Gets the temporary journal of items to add the next time the cache is rebuilt.
    /// </summary>
    protected List<AddJournalEntry> AddJournal { get; } = [];

    /// <summary>
    /// Gets the items in this collection, including items that have been filtered.
    /// </summary>
    protected List<T> Items { get; } = [];

    /// <summary>
    /// Gets the temporary journal of items to remove the next time the cache is rebuilt.
    /// </summary>
    protected List<int> RemoveJournal { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the cache should be rebuilt.
    /// </summary>
    protected bool ShouldRebuildCache { get; set; } = true;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">
    /// The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </param>
    public void Add(T item) {
        lock (this._lock) {
            this.AddJournal.Add(new AddJournalEntry(this.AddJournal.Count, item));
            this.ShouldRebuildCache = true;
            this.SubscribeToItemEvents(item);
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
                this.AddJournal.Add(new AddJournalEntry(this.AddJournal.Count, item));
            }

            this.ShouldRebuildCache = true;
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
            foreach (var t in this.Items) {
                this.UnsubscribeFromItemEvents(t);
            }

            this.AddJournal.Clear();
            this.RemoveJournal.Clear();
            this.Items.Clear();
            this.ShouldRebuildCache = true;
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, this.Items));
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
    public bool Contains(T item) => this.Items.Contains(item);

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
        this.Items.CopyTo(array, arrayIndex);
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
            if (this.ShouldRebuildCache) {
                this.ProcessRemoveJournal();
                this.ProcessAddJournal();

                // Rebuild the cache
                this._cachedFilteredItems.Clear();
                foreach (var item in this.Items) {
                    if (this._filter(item)) {
                        this._cachedFilteredItems.Add(item);
                    }
                }

                this.ShouldRebuildCache = false;
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
            if (this.AddJournal.Remove(AddJournalEntry.CreateKey(item))) {
                this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, item));
                return true;
            }

            var index = this.Items.IndexOf(item);
            if (index >= 0) {
                this.UnsubscribeFromItemEvents(item);
                this.RemoveJournal.Add(index);
                this.ShouldRebuildCache = true;
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
    /// Called when an item's property changes.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="propertyName">The property name.</param>
    protected virtual void OnItemPropertyChanged(T item, string propertyName) {
        if (propertyName == this._filterPropertyName) {
            this.ShouldRebuildCache = true;
        }
    }

    /// <summary>
    /// Processes the added entries since the last time the cache has been rebuilt.
    /// </summary>
    protected virtual void ProcessAddJournal() {
        if (this.AddJournal.Count == 0) {
            return;
        }

        foreach (var journal in this.AddJournal) {
            this.Items.Add(journal.Item);
            this.SubscribeToItemEvents(journal.Item);
        }

        this.AddJournal.Clear();
    }

    /// <summary>
    /// Processes the removed entries since the last time the cache has been rebuilt.
    /// </summary>
    protected virtual void ProcessRemoveJournal() {
        if (this.RemoveJournal.Count == 0) {
            return;
        }

        // Remove items in reverse. (Technically there exist faster ways to bulk-remove from a
        // variable-length array, but List<T> does not provide such a method.)
        this.RemoveJournal.Sort(this._removeJournalSortComparison);
        foreach (var t in this.RemoveJournal) {
            this.Items.RemoveAt(t);
        }

        this.RemoveJournal.Clear();
    }

    /// <summary>
    /// Subscribes to property changed events.
    /// </summary>
    /// <param name="item">The item.</param>
    protected void SubscribeToItemEvents(T item) {
        item.PropertyChanged += this.Item_PropertyChanged;
    }

    /// <summary>
    /// Unsubscribes from property changed events.
    /// </summary>
    /// <param name="item">The item.</param>
    protected void UnsubscribeFromItemEvents(T item) {
        item.PropertyChanged -= this.Item_PropertyChanged;
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Items).GetEnumerator();

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is T item && !string.IsNullOrEmpty(e.PropertyName)) {
            lock (this._lock) {
                this.OnItemPropertyChanged(item, e.PropertyName);
            }
        }
    }

    /// <summary>
    /// Add journal entry.
    /// </summary>
    protected record AddJournalEntry(int Order, T Item) {

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
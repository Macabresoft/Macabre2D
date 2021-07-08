namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// The FilterSortCollection class provides efficient, reusable sorting and filtering based on a
    /// configurable sort comparer, filter predicate, and associate change events. I stole all of
    /// this code from the MonoGame source code. Theirs is private to Game, I wanted to reuse this
    /// all over the place.
    /// </summary>
    public sealed class FilterSortCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : INotifyPropertyChanged {

        private static readonly Comparison<int> RemoveJournalSortComparison =
            (x, y) => Comparer<int>.Default.Compare(y, x);

        private readonly List<AddJournalEntry> _addJournal = new List<AddJournalEntry>();
        private readonly Comparison<AddJournalEntry> _addJournalSortComparison;
        private readonly List<T> _cachedFilteredItems = new List<T>();
        private readonly Predicate<T> _filter;
        private readonly string _filterPropertyName;
        private readonly List<T> _items = new List<T>();
        private readonly object _lock = new object();
        private readonly List<int> _removeJournal = new List<int>();
        private readonly Comparison<T> _sort;
        private readonly string _sortPropertyName;
        private bool _shouldRebuildCache = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSortCollection{T}" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="filterPropertyName">Name of the filter property.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="sortPropertName">Name of the sort propert.</param>
        public FilterSortCollection(
            Predicate<T> filter,
            string filterPropertyName,
            Comparison<T> sort,
            string sortPropertName) {
            this._filter = filter;
            this._filterPropertyName = filterPropertyName;
            this._sort = sort;
            this._sortPropertyName = sortPropertName;

            this._addJournalSortComparison = this.CompareAddJournalEntry;
        }

        /// <summary>
        /// Occurs when [collection changed].
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<T>>? CollectionChanged;

        /// <summary>
        /// Gets the number of elements contained in the <see
        /// cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count {
            get { return this._cachedFilteredItems.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return false; }
        }

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
        public void AddRange(IEnumerable<T> items) {
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
                for (var i = 0; i < this._items.Count; ++i) {
                    this.UnsubscribeFromItemEvents(this._items[i]);
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
        /// true if <paramref name="item" /> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(T item) {
            return this._items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to
        /// an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array" /> that is the destination of the
        /// elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see
        /// cref="T:System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex) {
            this._items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Iterates through each filtered item.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ForEachFilteredItem(Action<T> action) {
            this.RebuildCache();

            for (var i = 0; i < this._cachedFilteredItems.Count; i++) {
                action(this._cachedFilteredItems[i]);
            }

            // If the cache was invalidated as a result of processing items, now is a good time to
            // clear it and give the GC (more of) a chance to do its thing.
            if (this._shouldRebuildCache) {
                this._cachedFilteredItems.Clear();
            }
        }

        /// <summary>
        /// Iterates through each filtered item.
        /// </summary>
        /// <typeparam name="TUserData">The type of the user data.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="userData">The user data.</param>
        public void ForEachFilteredItem<TUserData>(Action<T, TUserData> action, TUserData userData) {
            this.RebuildCache();

            for (var i = 0; i < this._cachedFilteredItems.Count; i++) {
                action(this._cachedFilteredItems[i], userData);
            }

            // If the cache was invalidated as a result of processing items, now is a good time to
            // clear it and give the GC (more of) a chance to do its thing.
            if (this._shouldRebuildCache) {
                this._cachedFilteredItems.Clear();
            }
        }

        /// <summary>
        /// Iterates through each filtered item.
        /// </summary>
        /// <typeparam name="TUserData">The type of the user data.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="userData">The user data.</param>
        public void ForEachFilteredItem<TUserData1, TUserData2>(Action<T, TUserData1, TUserData2> action, TUserData1 userData1, TUserData2 userData2) {
            this.RebuildCache();

            for (var i = 0; i < this._cachedFilteredItems.Count; i++) {
                action(this._cachedFilteredItems[i], userData1, userData2);
            }

            // If the cache was invalidated as a result of processing items, now is a good time to
            // clear it and give the GC (more of) a chance to do its thing.
            if (this._shouldRebuildCache) {
                this._cachedFilteredItems.Clear();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() {
            this.RebuildCache();
            return this._cachedFilteredItems.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((System.Collections.IEnumerable)this._items).GetEnumerator();
        }

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
        /// Removes the first occurrence of a specific object from the <see
        /// cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also
        /// returns false if <paramref name="item" /> is not found in the original <see
        /// cref="T:System.Collections.Generic.ICollection`1" />.
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

        private int CompareAddJournalEntry(AddJournalEntry x, AddJournalEntry y) {
            var result = this._sort(x.Item, y.Item);
            if (result != 0) {
                return result;
            }

            return x.Order - y.Order;
        }

        private void InvalidateCache() {
            this._shouldRebuildCache = true;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == this._sortPropertyName && sender is T item) {
                lock (this._lock) {
                    var index = this._items.IndexOf(item);

                    this._addJournal.Add(new AddJournalEntry(this._addJournal.Count, item));
                    this._removeJournal.Add(index);

                    // Until the item is back in place, we don't care about its events. We will
                    // re-subscribe when this._addJournal is processed.
                    this.UnsubscribeFromItemEvents(item);
                    this.InvalidateCache();
                }
            }
            else if (e.PropertyName == this._filterPropertyName) {
                this.InvalidateCache();
            }
        }

        private void ProcessAddJournal() {
            if (this._addJournal.Count == 0) {
                return;
            }

            // Prepare the this._addJournal to be merge-sorted with this._items. this._items is
            // already sorted (because it is always sorted).
            this._addJournal.Sort(this._addJournalSortComparison);

            var iAddJournal = 0;
            var iItems = 0;

            while (iItems < this._items.Count && iAddJournal < this._addJournal.Count) {
                var addJournalItem = this._addJournal[iAddJournal].Item;
                // If addJournalItem is less than (belongs before) this._items[iItems], insert it.
                if (this._sort(addJournalItem, this._items[iItems]) < 0) {
                    this.SubscribeToItemEvents(addJournalItem);
                    this._items.Insert(iItems, addJournalItem);
                    iAddJournal++;
                }
                // Always increment iItems, either because we inserted and need to move past the
                // insertion, or because we didn't insert and need to consider the next element.
                iItems++;
            }

            // If this._addJournal had any "tail" items, append them all now.
            for (; iAddJournal < this._addJournal.Count; iAddJournal++) {
                var addJournalItem = this._addJournal[iAddJournal].Item;
                this.SubscribeToItemEvents(addJournalItem);
                this._items.Add(addJournalItem);
            }

            this._addJournal.Clear();
        }

        private void ProcessRemoveJournal() {
            if (this._removeJournal.Count == 0) {
                return;
            }

            // Remove items in reverse. (Technically there exist faster ways to bulk-remove from a
            // variable-length array, but List<T> does not provide such a method.)
            this._removeJournal.Sort(RemoveJournalSortComparison);
            for (var i = 0; i < this._removeJournal.Count; i++) {
                var index = this._removeJournal[i];
                if (index < this._items.Count) {
                    this._items.RemoveAt(this._removeJournal[i]);
                }
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
        private struct AddJournalEntry {

            /// <summary>
            /// The item.
            /// </summary>
            public readonly T Item;

            /// <summary>
            /// The order.
            /// </summary>
            public readonly int Order;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddJournalEntry" /> struct.
            /// </summary>
            /// <param name="order">Order.</param>
            /// <param name="item">Item.</param>
            public AddJournalEntry(int order, T item) {
                this.Order = order;
                this.Item = item;
            }

            /// <summary>
            /// Creates the key.
            /// </summary>
            /// <returns>The key.</returns>
            /// <param name="item">Item.</param>
            public static AddJournalEntry CreateKey(T item) {
                return new AddJournalEntry(-1, item);
            }

            /// <inheritdoc />
            public override bool Equals(object? obj) {
                if (!(obj is AddJournalEntry)) {
                    return false;
                }

                return object.Equals(this.Item, ((AddJournalEntry)obj).Item);
            }

            /// <inheritdoc />
            public override int GetHashCode() {
                return this.Item.GetHashCode();
            }
        }
    }
}
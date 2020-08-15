namespace Macabre2D.Framework {

    using Macabresoft.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The FilterCollection class provides efficient, reusable filtering based on a filter
    /// predicate, and associate change events.
    /// </summary>
    public sealed class FilterCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : INotifyPropertyChanged {

        private static readonly Comparison<int> RemoveJournalSortComparison =
            (x, y) => Comparer<int>.Default.Compare(y, x);

        private readonly List<T> _cachedFilteredItems = new List<T>();
        private readonly Predicate<T> _filter;
        private readonly string _filterPropertyName;
        private readonly List<T> _items = new List<T>();
        private readonly List<T> _itemsToAdd = new List<T>();
        private readonly List<int> _itemsToRemove = new List<int>();
        private bool _shouldRebuildCache = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Macabre2D.ComponentCollection`1" /> class.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <param name="filterPropertyName">Name of the filter property.</param>
        public FilterCollection(
            Predicate<T> filter,
            string filterPropertyName) {
            this._filter = filter;
            this._filterPropertyName = filterPropertyName;
        }

        /// <summary>
        /// Occurs when [collection changed].
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged;

        /// <summary>
        /// Gets the number of elements contained in the <see
        /// cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count {
            get { return this._items.Count; }
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
            this._itemsToAdd.Add(item);
            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(true, item));
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
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items) {
            this._itemsToAdd.AddRange(items);
            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(true, items));
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear() {
            for (var i = 0; i < this._items.Count; i++) {
                this.UnsubscribeFromItemEvents(this._items[i]);
            }

            // Copy before clear for collection changed event.
            var items = this._items.ToList();
            this._itemsToAdd.Clear();
            this._itemsToRemove.Clear();
            this._items.Clear();
            this.InvalidateCache();
            this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, items));
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
        /// Iterates through each filtered item asynchronously.
        /// </summary>
        /// <typeparam name="TUserData">The type of the user data.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="userData">The user data.</param>
        public Task ForeachEachFilteredItemAsync<TUserData>(Func<T, TUserData, Task> action, TUserData userData) {
            return Task.Run(() => {
                this.RebuildCache();

                var tasks = new Task[this._cachedFilteredItems.Count];

                for (var i = 0; i < this._cachedFilteredItems.Count; i++) {
                    tasks[i] = action(this._cachedFilteredItems[i], userData);
                }

                Task.WaitAll(tasks);

                // If the cache was invalidated as a result of processing items, now is a good time
                // to clear it and give the GC (more of) a chance to do its thing.
                if (this._shouldRebuildCache) {
                    this._cachedFilteredItems.Clear();
                }
            });
        }

        /// <summary>
        /// Iterates through each filtered item asynchronously.
        /// </summary>
        /// <typeparam name="TUserData">The type of the user data.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="userData">The user data.</param>
        public Task ForeachEachFilteredItemAsync<TUserData1, TUserDate2>(Func<T, TUserData1, TUserDate2, Task> action, TUserData1 userData1, TUserDate2 userDate2) {
            return Task.Run(() => {
                this.RebuildCache();

                var tasks = new Task[this._cachedFilteredItems.Count];

                for (var i = 0; i < this._cachedFilteredItems.Count; i++) {
                    tasks[i] = action(this._cachedFilteredItems[i], userData1, userDate2);
                }

                Task.WaitAll(tasks);

                // If the cache was invalidated as a result of processing items, now is a good time
                // to clear it and give the GC (more of) a chance to do its thing.
                if (this._shouldRebuildCache) {
                    this._cachedFilteredItems.Clear();
                }
            });
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
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() {
            return this._items.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns>The enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((System.Collections.IEnumerable)this._items).GetEnumerator();
        }

        /// <summary>
        /// Rebuilds the cache.
        /// </summary>
        public void RebuildCache() {
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
            if (this._itemsToAdd.Remove(item)) {
                this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, item));
                return true;
            }

            var index = this._items.IndexOf(item);
            if (index >= 0) {
                this.UnsubscribeFromItemEvents(item);
                this._itemsToRemove.Add(index);
                this.InvalidateCache();
                this.CollectionChanged.SafeInvoke(this, new CollectionChangedEventArgs<T>(false, item));
                return true;
            }

            return false;
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

        private void InvalidateCache() {
            this._shouldRebuildCache = true;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == this._filterPropertyName) {
                this.InvalidateCache();
            }
        }

        private void ProcessAddJournal() {
            if (this._itemsToAdd.Count == 0) {
                return;
            }

            foreach (var item in this._itemsToAdd) {
                this.SubscribeToItemEvents(item);
                this._items.Add(item);
            }

            this._itemsToAdd.Clear();
        }

        private void ProcessRemoveJournal() {
            if (this._itemsToRemove.Count == 0) {
                return;
            }

            // Remove items in reverse. (Technically there exist faster ways to bulk-remove from a
            // variable-length array, but List<T> does not provide such a method.)
            this._itemsToRemove.Sort(RemoveJournalSortComparison);
            for (var i = 0; i < this._itemsToRemove.Count; ++i) {
                this._items.RemoveAt(this._itemsToRemove[i]);
            }

            this._itemsToRemove.Clear();
        }

        private void SubscribeToItemEvents(T item) {
            item.PropertyChanged += this.Item_PropertyChanged;
        }

        private void UnsubscribeFromItemEvents(T item) {
            item.PropertyChanged -= this.Item_PropertyChanged;
        }
    }
}
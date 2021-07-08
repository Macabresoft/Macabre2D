namespace Macabresoft.Macabre2D.Framework {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Event arguments for a collection changed event.
    /// </summary>
    /// <typeparam name="T">The type contained in the collection.</typeparam>
    /// <seealso cref="EventArgs"/>
    public sealed class CollectionChangedEventArgs<T> : EventArgs {

        /// <summary>
        /// The items that were either added or removed.
        /// </summary>
        public readonly IEnumerable<T> Items;

        /// <summary>
        /// A value indicating whether or not items were added. If true, this event was for items
        /// being added; otherwise, items were removed.
        /// </summary>
        public readonly bool ItemsWereAdded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="itemsWereAdded">If set to <c>true</c> [items were added].</param>
        /// <param name="items">The items.</param>
        public CollectionChangedEventArgs(bool itemsWereAdded, IEnumerable<T> items) {
            this.ItemsWereAdded = itemsWereAdded;
            this.Items = items;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="itemsWereAdded">If set to <c>true</c> [items were added].</param>
        /// <param name="items">The items.</param>
        public CollectionChangedEventArgs(bool itemsWereAdded, params T[] items) : this(itemsWereAdded, items as IEnumerable<T>) {
        }
    }
}
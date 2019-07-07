namespace Macabre2D.Framework {

    using System.Collections.Generic;

    /// <summary>
    /// Extensions for anything that implements <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensions {

        /// <summary>
        /// Adds a range of items to a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="itemsToAdd">The items to add.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd) {
            foreach (var item in itemsToAdd) {
                collection.Add(item);
            }
        }
    }
}
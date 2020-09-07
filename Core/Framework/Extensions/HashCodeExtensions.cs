namespace Macabresoft.MonoGame.Core {

    using System.Collections.Generic;

    /// <summary>
    /// Extensions for hash codes.
    /// </summary>
    public static class HashCodeExtensions {

        /// <summary>
        /// The hash code starting value.
        /// </summary>
        public const int HashCodeStart = 560318066;

        /// <summary>
        /// Returns a combined hash code for this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="currentHashCode">The current hash code.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public static int GetCombinedHashCode<T>(this T value, int currentHashCode) {
            return currentHashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(value);
        }

        /// <summary>
        /// Returns a combined hash code for this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public static int GetCombinedHashCode<T>(this T value) {
            return value.GetCombinedHashCode(HashCodeStart);
        }
    }
}
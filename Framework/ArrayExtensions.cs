namespace Macabre2D.Framework {

    /// <summary>
    /// Extensions for arrays.
    /// </summary>
    public static class ArrayExtensions {

        /// <summary>
        /// Populates the entire array with the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value.</param>
        public static void Populate<T>(this T[] array, T value) {
            for (var i = 0; i < array.Length; i++) {
                array[i] = value;
            }
        }
    }
}
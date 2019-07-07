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

        /// <summary>
        /// Resizes the two dimensional array to the new column and row count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rows">The rows.</param>
        /// <returns>The resized array.</returns>
        public static T[,] Resize<T>(this T[,] array, int columns, int rows) {
            var originalColumns = array.GetLength(0);
            var originalRows = array.GetLength(1);
            var newArray = new T[columns, rows];

            for (var x = 0; x < columns; x++) {
                for (var y = 0; y < rows; y++) {
                    if (x < originalColumns && y < originalRows) {
                        newArray[x, y] = array[x, y];
                    }
                    else {
                        newArray[x, y] = default;
                    }
                }
            }

            return newArray;
        }
    }
}
namespace Macabre2D.Framework {

    /// <summary>
    /// Extension methods to handle math.
    /// </summary>
    public static class MathExtensions {

        /// <summary>
        /// Determines whether the value is a power of two.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is a power of two; otherwise, <c>false</c>.</returns>
        public static bool IsPowerOfTwo(this int value) {
            return (value != 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Converts a value to negative one or one.
        /// </summary>
        /// <remarks>Zero is treated as positive in this case.</remarks>
        /// <param name="value">The value.</param>
        /// <returns>The sign.</returns>
        public static float ToSign(this float value) {
            return value < 0 ? -1f : 1f;
        }
    }
}
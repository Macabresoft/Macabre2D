namespace Macabre2D.Framework.Extensions {

    /// <summary>
    /// Extension methods for <see cref="float"/>.
    /// </summary>
    public static class FloatExtensions {

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
namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Determines whether the text contains the provided value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="stringComparisonType">Type of the string comparison.</param>
        /// <returns><c>true</c> if the text contains the specified value; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string text, string value, StringComparison stringComparisonType) {
            return text.IndexOf(value, stringComparisonType) >= 0;
        }
    }
}
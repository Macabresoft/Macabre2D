namespace Macabre2D.Framework {

    using System.Linq;

    /// <summary>
    /// Extensions for strings that deal with files.
    /// </summary>
    public static class FileStringExtensions {

        /// <summary>
        /// Converts a string to a file safe string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The file safe string.</returns>
        public static string ToSafeString(this string value) {
            if (value != null) {
                return new string(value.Where(x => char.IsLetterOrDigit(x)).ToArray());
            }

            return string.Empty;
        }
    }
}
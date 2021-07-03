namespace Macabresoft.Macabre2D.UI.Common.Utilities {
    using System.IO;

    /// <summary>
    /// A helper class for handling file based operations.
    /// </summary>
    public static class FileHelper {
        
        /// <summary>
        /// Gets a value indicating whether or not the provided file name is valid.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A value indicating whether or not the provided file name is valid.</returns>
        public static bool IsValidFileName(string fileName) {
            return !string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidPathChars()) < 0;
        }
    }
}
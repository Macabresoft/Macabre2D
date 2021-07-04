namespace Macabresoft.Macabre2D.UI.Common.Utilities {
    using System.IO;
    using System.Linq;

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
            return !string.IsNullOrEmpty(fileName) && Path.GetInvalidPathChars().Any(fileName.Contains);
        }
    }
}
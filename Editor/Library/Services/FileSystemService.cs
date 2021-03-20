namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Interface for a service which wraps basic file system operations.
    /// </summary>
    /// <remarks>Exists purely for unit testing purposes, which feels wrong, but I don't care.</remarks>
    public interface IFileSystemService {
        /// <summary>
        /// Creates the directory structure for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Returns a value indicating whether a directory exists at the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A value indicating whether a directory exists at the given path.</returns>
        bool DoesDirectoryExist(string path);

        /// <summary>
        /// Returns a value indicating whether a file exists at the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A value indicating whether a file exists at the given path.</returns>
        bool DoesFileExist(string path);

        /// <summary>
        /// Gets all directories in the specified directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>All directories in the specified directory.</returns>
        IEnumerable<string> GetDirectories(string directoryPath);

        /// <summary>
        /// Gets all directories in the specified directory that match the search pattern.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>All directories in the specified directory that match the search pattern.</returns>
        IEnumerable<string> GetDirectories(string directoryPath, string searchPattern);

        /// <summary>
        /// Gets all files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>All files in the specified directory.</returns>
        IEnumerable<string> GetFiles(string directoryPath);

        /// <summary>
        /// Gets all files in the specified directory that match the search pattern.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>All files in the specified directory that match the search pattern.</returns>
        IEnumerable<string> GetFiles(string directoryPath, string searchPattern);

        /// <summary>
        /// Moves a directory from the original path to the new path.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        void MoveDirectory(string originalPath, string newPath);

        /// <summary>
        /// Moves a file from the original path to the new path.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        void MoveFile(string originalPath, string newPath);
    }

    /// <summary>
    /// A service which wraps basic file system operations.
    /// </summary>
    public class FileSystemService : IFileSystemService {
        /// <inheritdoc />
        public void CreateDirectory(string path) {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public bool DoesDirectoryExist(string path) {
            return Directory.Exists(path);
        }

        /// <inheritdoc />
        public bool DoesFileExist(string path) {
            return File.Exists(path);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string directoryPath) {
            return Directory.GetDirectories(directoryPath);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string directoryPath, string searchPattern) {
            return Directory.GetDirectories(directoryPath, searchPattern);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string directoryPath) {
            return Directory.GetFiles(directoryPath);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string directoryPath, string searchPattern) {
            return Directory.GetFiles(directoryPath, searchPattern);
        }

        /// <inheritdoc />
        public void MoveDirectory(string originalPath, string newPath) {
            Directory.Move(originalPath, newPath);
        }

        /// <inheritdoc />
        public void MoveFile(string originalPath, string newPath) {
            File.Move(originalPath, newPath);
        }
    }
}
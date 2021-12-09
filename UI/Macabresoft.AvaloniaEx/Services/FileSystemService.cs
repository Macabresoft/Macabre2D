namespace Macabresoft.AvaloniaEx;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Platform;

/// <summary>
/// Interface for a service which wraps basic file system operations.
/// </summary>
/// <remarks>Exists purely for unit testing purposes, which feels wrong, but I don't care.</remarks>
public interface IFileSystemService {
    /// <summary>
    /// Copies the file from its original path to the new path.
    /// </summary>
    /// <param name="originalPath">The original path.</param>
    /// <param name="newPath">The new path.</param>
    void CopyFile(string originalPath, string newPath);

    /// <summary>
    /// Creates the directory structure for the given path.
    /// </summary>
    /// <param name="path">The path.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Deletes the specified directory and all of its children.
    /// </summary>
    /// <param name="path">The path to the directory.</param>
    void DeleteDirectory(string path);

    /// <summary>
    /// Deletes the specified file and all of its children.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    void DeleteFile(string path);

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
    /// Gets a value indicating whether or not the provided name is valid for a file or directory name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A value indicating whether or not the provided name is valid for a file or directory name.</returns>
    bool IsValidFileOrDirectoryName(string name);

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

    /// <summary>
    /// Opens a directory in the file explorer.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    void OpenDirectoryInFileExplorer(string directoryPath);

    /// <summary>
    /// Writes all text to the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="text">The text.</param>
    void WriteAllText(string filePath, string text);
}

/// <summary>
/// A service which wraps basic file system operations.
/// </summary>
public class FileSystemService : IFileSystemService {
    /// <inheritdoc />
    public void CopyFile(string originalPath, string newPath) {
        File.Copy(originalPath, newPath);
    }

    /// <inheritdoc />
    public void CreateDirectory(string path) {
        Directory.CreateDirectory(path);
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path) {
        if (this.DoesDirectoryExist(path)) {
            Directory.Delete(path, true);
        }
    }

    /// <inheritdoc />
    public void DeleteFile(string path) {
        if (this.DoesFileExist(path)) {
            File.Delete(path);
        }
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
    public bool IsValidFileOrDirectoryName(string name) {
        return !string.IsNullOrEmpty(name) && name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;
    }

    /// <inheritdoc />
    public void MoveDirectory(string originalPath, string newPath) {
        Directory.Move(originalPath, newPath);
    }

    /// <inheritdoc />
    public void MoveFile(string originalPath, string newPath) {
        File.Move(originalPath, newPath);
    }

    /// <inheritdoc />
    public void OpenDirectoryInFileExplorer(string directoryPath) {
        if (this.DoesDirectoryExist(directoryPath)) {
            try {
                var runtimeInfo = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo();
                var program = runtimeInfo.OperatingSystem switch {
                    OperatingSystemType.WinNT => "explorer.exe",
                    OperatingSystemType.OSX => "open",
                    OperatingSystemType.Linux => "xdg-open",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(program)) {
                    var startInfo = new ProcessStartInfo(program) {
                        ArgumentList = { directoryPath }
                    };

                    Process.Start(startInfo);
                }
            }
            catch {
                // its really not worth throwing an exception because the file explorer didn't open
            }
        }
    }

    /// <inheritdoc />
    public void WriteAllText(string filePath, string text) {
        File.WriteAllText(filePath, text);
    }
}
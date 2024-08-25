namespace Macabresoft.Macabre2D.Common;

/// <summary>
/// Interface for a data manager.
/// </summary>
public interface IDataManager {
    /// <summary>
    /// Deletes the specified save data.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    void Delete(string fileName);

    /// <summary>
    /// Gets the files in the data directory that have the specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns>All files with the specified extension.</returns>
    IEnumerable<string> GetFiles(string fileExtension);

    /// <summary>
    /// Gets the path to the data directory.
    /// </summary>
    /// <returns>The path to the data directory.</returns>
    string GetPathToDataDirectory();

    /// <summary>
    /// Saves the specified save data.
    /// </summary>
    /// <typeparam name="T">Versioned data.</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="saveData">The save data.</param>
    void Save<T>(string fileName, T saveData) where T : IVersionedData;

    /// <summary>
    /// Tries to load the specified data.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="data">The loaded data.</param>
    /// <returns>A value indicating whether the data was found.</returns>
    bool TryLoad<T>(string fileName, out T? data) where T : class, IVersionedData;
}
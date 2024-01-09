namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Interface for a save data manager.
/// </summary>
public interface ISaveDataManager {
    /// <summary>
    /// Deletes the specified save data.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="projectName">The project name.</param>
    void Delete(string fileName, string projectName);

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
    /// <param name="projectName">The project name.</param>
    /// <param name="saveData">The save data.</param>
    void Save<T>(string fileName, string projectName, T saveData) where T : IVersionedData;

    /// <summary>
    /// Tries to load the specified data.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="projectName">The project name.</param>
    /// <param name="data">The loaded data.</param>
    /// <returns>A value indicating whether or not the data was found.</returns>
    bool TryLoad<T>(string fileName, string projectName, out T? data) where T : class, IVersionedData;
}

/// <summary>
/// Static class that provides utility for <see cref="ISaveDataManager" />.
/// </summary>
public static class SaveDataManager {
    /// <summary>
    /// Gets the empty.
    /// </summary>
    /// <value>The empty.</value>
    public static ISaveDataManager Empty { get; } = new EmptySaveDataManager();
}
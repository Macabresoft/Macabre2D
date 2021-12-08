namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// An empty save data manager that does nothing with the file system.
/// </summary>
internal class EmptySaveDataManager : ISaveDataManager {
    /// <inheritdoc />
    public void Delete(string fileName, string projectName) {
    }

    /// <inheritdoc />
    public string GetPathToDataDirectory() {
        return string.Empty;
    }

    /// <inheritdoc />
    public void Save<T>(string fileName, string projectName, T saveData) where T : IVersionedData {
    }

    /// <inheritdoc />
    public bool TryLoad<T>(string fileName, string projectName, out T? data) where T : class, IVersionedData {
        data = default;
        return false;
    }
}
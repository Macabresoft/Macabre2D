namespace Macabresoft.Macabre2D.Common;

/// <summary>
/// An empty save data manager that does nothing with the file system.
/// </summary>
public class EmptyDataManager : IDataManager {
    /// <summary>
    /// Gets an instance of <see cref="EmptyDataManager"/>.
    /// </summary>
    public static IDataManager Instance { get; } = new EmptyDataManager();
    
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
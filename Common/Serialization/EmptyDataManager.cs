namespace Macabresoft.Macabre2D.Common;

/// <summary>
/// An empty save data manager that does nothing with the file system.
/// </summary>
public class EmptyDataManager : IDataManager {
    /// <summary>
    /// Gets an instance of <see cref="EmptyDataManager" />.
    /// </summary>
    public static IDataManager Instance { get; } = new EmptyDataManager();

    /// <inheritdoc />
    public void Delete(string fileName) {
    }

    public IEnumerable<string> GetFiles(string fileExtension) => [];

    /// <inheritdoc />
    public string GetPathToDataDirectory() => string.Empty;

    /// <inheritdoc />
    public void Initialize(string companyName, string projectName) {
    }

    /// <inheritdoc />
    public void Save<T>(string fileName, T saveData) where T : IVersionedData {
    }

    /// <inheritdoc />
    public bool TryLoad<T>(string fileName, out T? data) where T : class, IVersionedData {
        data = default;
        return false;
    }
}
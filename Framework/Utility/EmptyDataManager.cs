namespace Macabresoft.Macabre2D.Framework;

using Macabresoft.Macabre2D.Common;

/// <summary>
/// An empty save data manager that does nothing with the file system.
/// </summary>
internal class EmptyDataManager : IDataManager {
    /// <summary>
    /// Gets the empty.
    /// </summary>
    /// <value>The empty.</value>
    public static IDataManager Empty { get; } = new EmptyDataManager();
    
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
namespace Macabresoft.Macabre2D.Common;

using Macabresoft.Core;

/// <summary>
/// Manages the loading and saving of data in Windows.
/// </summary>
public sealed class WindowsDataManager : IDataManager {
    /// <inheritdoc />
    public void Delete(string fileName, string projectName) {
        var filePath = this.GetFilePath(fileName, projectName);
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFiles(string fileExtension) => Directory.EnumerateFiles(this.GetPathToDataDirectory(), $"*{fileExtension}", SearchOption.TopDirectoryOnly);

    /// <inheritdoc />
    public string GetPathToDataDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <inheritdoc />
    public void Save<T>(string fileName, string projectName, T saveData) where T : IVersionedData {
        Serializer.Instance.Serialize(saveData, this.GetFilePath(fileName, projectName));
    }

    /// <inheritdoc />
    public bool TryLoad<T>(string fileName, string projectName, out T? loadedData) where T : class, IVersionedData {
        var filePath = this.GetFilePath(fileName, projectName);
        var result = true;
        try {
            loadedData = Serializer.Instance.Deserialize<T>(filePath);
        }
        catch {
            result = false;
            loadedData = default;
        }

        return result;
    }

    private string GetFilePath(string fileName, string projectName) => Path.Combine(this.GetPathToDataDirectory(), projectName.ToSafeString(), fileName);
}
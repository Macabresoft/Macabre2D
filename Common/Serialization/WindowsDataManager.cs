namespace Macabresoft.Macabre2D.Common;

using Macabresoft.Core;

/// <summary>
/// Manages the loading and saving of data in Windows.
/// </summary>
public sealed class WindowsDataManager : IDataManager {
    private const string CosmicJamDirectory = "Cosmic Jam";
    private const string MacabresoftDirectory = "Macabresoft";

    /// <inheritdoc />
    public void Delete(string fileName) {
        var filePath = this.GetFilePath(fileName);
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFiles(string fileExtension) => Directory.EnumerateFiles(this.GetPathToDataDirectory(), $"*{fileExtension}", SearchOption.TopDirectoryOnly);

    /// <inheritdoc />
    public string GetPathToDataDirectory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), MacabresoftDirectory, CosmicJamDirectory);

    /// <inheritdoc />
    public void Save<T>(string fileName, T saveData) where T : IVersionedData {
        Serializer.Instance.Serialize(saveData, this.GetFilePath(fileName));
    }

    /// <inheritdoc />
    public bool TryLoad<T>(string fileName, out T? loadedData) where T : class, IVersionedData {
        var filePath = this.GetFilePath(fileName);
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

    private string GetFilePath(string fileName) => Path.Combine(this.GetPathToDataDirectory(), fileName);
}
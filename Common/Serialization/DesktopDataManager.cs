namespace Macabre2D.Common;

/// <summary>
/// Manages the loading and saving of data on a desktop PC.
/// </summary>
public sealed class DesktopDataManager : IDataManager {
    private string _companyName = string.Empty;
    private string _projectName = string.Empty;

    /// <inheritdoc />
    public void Delete(string fileName) {
        var filePath = this.GetFilePath(fileName);
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetFiles(string fileExtension) {
        var directory = this.GetPathToDataDirectory();
        return Directory.Exists(directory) ? Directory.EnumerateFiles(directory, $"*{fileExtension}", SearchOption.TopDirectoryOnly) : [];
    }

    /// <inheritdoc />
    public string GetPathToDataDirectory() {
        var isCompanyEmpty = string.IsNullOrEmpty(this._companyName);
        var isProjectEmpty = string.IsNullOrEmpty(this._projectName);
        if (!isCompanyEmpty && !isProjectEmpty) {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this._companyName, this._projectName);
        }

        if (isProjectEmpty) {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this._companyName);
        }

        if (isCompanyEmpty) {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this._projectName);
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }

    /// <inheritdoc />
    public void Initialize(string companyName, string projectName) {
        this._companyName = companyName;
        this._projectName = projectName;
    }

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
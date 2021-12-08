namespace Macabresoft.Macabre2D.Framework;

using System;
using System.IO;
using Macabresoft.Core;

/// <summary>
/// Manages the loading and saving of save data.
/// </summary>
public sealed class WindowsSaveDataManager : ISaveDataManager {
    /// <inheritdoc />
    public void Delete(string fileName, string projectName) {
        var filePath = this.GetFilePath(fileName, projectName);
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
    }

    /// <inheritdoc />
    public string GetPathToDataDirectory() {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }

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

    private string GetFilePath(string fileName, string projectName) {
        return Path.Combine(this.GetPathToDataDirectory(), projectName.ToSafeString(), fileName);
    }
}
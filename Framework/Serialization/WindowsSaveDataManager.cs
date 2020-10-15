namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using System;
    using System.IO;

    /// <summary>
    /// Manages the loading and saving of save data.
    /// </summary>
    public sealed class WindowsSaveDataManager : ISaveDataManager {

        /// <inheritdoc />
        public void Delete(string fileName) {
            var filePath = this.GetFilePath(fileName);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }

        /// <inheritdoc />
        public string GetPathToDataDirectory() {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        /// <inheritdoc />
        public T Load<T>(string fileName) where T : IVersionedData {
            var filePath = this.GetFilePath(fileName);
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException(filePath);
            }

            return Serializer.Instance.Deserialize<T>(filePath);
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

        private string GetFilePath(string fileName) {
            return Path.Combine(this.GetPathToDataDirectory(), GameSettings.Instance.ProjectName.ToSafeString(), fileName);
        }
    }
}
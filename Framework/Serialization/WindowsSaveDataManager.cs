namespace Macabre2D.Framework {

    using System;
    using System.IO;

    /// <summary>
    /// Manages the loading and saving of save data.
    /// </summary>
    public sealed class WindowsSaveDataManager {

        /// <summary>
        /// Deletes the specified save data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Delete(string fileName) {
            var filePath = WindowsSaveDataManager.GetFilePath(fileName);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Loads the specified save data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The save data.</returns>
        public T Load<T>(string fileName) where T : IVersionedData {
            var filePath = WindowsSaveDataManager.GetFilePath(fileName);
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException(filePath);
            }

            return Serializer.Instance.Deserialize<T>(WindowsSaveDataManager.GetFilePath(fileName));
        }

        /// <summary>
        /// Saves the specified save data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="saveData">The save data.</param>
        public void Save<T>(string fileName, T saveData) where T : IVersionedData {
            Serializer.Instance.Serialize(saveData, WindowsSaveDataManager.GetFilePath(fileName));
        }

        private static string GetFilePath(string fileName) {
            // TODO: platforms other than windows
            var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataDirectory, GameSettings.Instance.ProjectName.ToSafeString(), fileName);
        }
    }
}
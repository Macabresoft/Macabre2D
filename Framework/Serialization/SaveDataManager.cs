namespace Macabre2D.Framework {

    using System;
    using System.IO;

    /// <summary>
    /// Managers the loading and saving of save data.
    /// </summary>
    public sealed class SaveDataManager {
        private readonly Serializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveDataManager"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="ArgumentNullException">serializer</exception>
        public SaveDataManager(Serializer serializer) {
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Deletes the specified save data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Delete(string fileName) {
            var filePath = this.GetFilePath(fileName);
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
        public T Load<T>(string fileName) {
            var filePath = this.GetFilePath(fileName);
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException(filePath);
            }

            return this._serializer.Deserialize<T>(this.GetFilePath(fileName));
        }

        /// <summary>
        /// Saves the specified save data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="saveData">The save data.</param>
        public void Save<T>(string fileName, T saveData) {
            this._serializer.Serialize(saveData, this.GetFilePath(fileName));
        }

        private string GetFilePath(string fileName) {
            // TODO: platforms other than windows
            var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataDirectory, GameSettings.Instance.ProjectName.ToSafeString(), fileName);
        }
    }
}
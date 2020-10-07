namespace Macabresoft.MonoGame.Core2D {

    /// <summary>
    /// An empty save data manager that does nothing with the file system.
    /// </summary>
    internal class EmptySaveDataManager : ISaveDataManager {

        /// <inheritdoc />
        public void Delete(string fileName) {
            return;
        }

        /// <inheritdoc />
        public string GetPathToDataDirectory() {
            return string.Empty;
        }

        /// <inheritdoc />
        public void Save<T>(string fileName, T saveData) where T : IVersionedData {
            return;
        }

        /// <inheritdoc />
        public bool TryLoad<T>(string fileName, out T? data) where T : class, IVersionedData {
            data = default;
            return false;
        }
    }
}
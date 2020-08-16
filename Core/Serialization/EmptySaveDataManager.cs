namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// An empty save data manager that does nothing with the file system.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.ISaveDataManager"/>
    public class EmptySaveDataManager : ISaveDataManager {

        /// <inheritdoc/>
        public void Delete(string fileName) {
            return;
        }

        /// <inheritdoc/>
        public string GetPathToDataDirectory() {
            return null;
        }

        /// <inheritdoc/>
        public T Load<T>(string fileName) where T : IVersionedData {
            return default;
        }

        /// <inheritdoc/>
        public void Save<T>(string fileName, T saveData) where T : IVersionedData {
            return;
        }

        /// <inheritdoc/>
        public bool TryLoad<T>(string fileName, out T loadedData) where T : IVersionedData {
            loadedData = default;
            return false;
        }
    }
}
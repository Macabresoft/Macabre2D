namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for a save data mananger.
    /// </summary>
    public interface ISaveDataManager {

        /// <summary>
        /// Deletes the specified save data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void Delete(string fileName);

        /// <summary>
        /// Gets the path to the data directory.
        /// </summary>
        /// <returns>The path to the data directory.</returns>
        string GetPathToDataDirectory();

        /// <summary>
        /// Loads the specified save data.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The save data.</returns>
        T Load<T>(string fileName) where T : IVersionedData;

        /// <summary>
        /// Saves the specified save data.
        /// </summary>
        /// <typeparam name="T">Versioned data.</typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="saveData">The save data.</param>
        void Save<T>(string fileName, T saveData) where T : IVersionedData;

        /// <summary>
        /// Tries to load the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="loadedData">The loaded data.</param>
        /// <returns>A value indicating whether or not the data was found.</returns>
        bool TryLoad<T>(string fileName, out T loadedData) where T : IVersionedData;
    }
}
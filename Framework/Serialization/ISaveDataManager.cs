namespace Macabre2D.Framework {

    public interface ISaveDataManager {

        /// <summary>
        /// Deletes the specified save data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void Delete(string fileName);

        /// <summary>
        /// Loads the specified save data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
    }
}
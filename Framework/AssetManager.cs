namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface to manage assets.
    /// </summary>
    public interface IAssetManager {

        /// <summary>
        /// Clears the mappings.
        /// </summary>
        void ClearMappings();

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The path.</returns>
        string GetPath(Guid id);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void Initialize(ContentManager contentManager);

        /// <summary>
        /// Loads the asset at the specified path.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>The asset.</returns>
        T Load<T>(string path);

        /// <summary>
        /// Loads the asset with the specified identifier.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="Id">The identifier.</param>
        /// <returns>The asset.</returns>
        T Load<T>(Guid id);

        /// <summary>
        /// Sets the mapping.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="contentPath">The content path.</param>
        void SetMapping(Guid id, string contentPath);

        /// <summary>
        /// Unloads the content manager.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Unload();

        /// <summary>
        /// Unloads the asset with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Unload(Guid id);

        /// <summary>
        /// Unloads the asset with the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void Unload(string path);
    }

    /// <summary>
    /// Maps content with identifiers. This should be the primary way that content is accessed.
    /// </summary>
    [DataContract]
    public sealed class AssetManager : IAssetManager {
        internal const string ContentFileName = "AssetManager";

        private static IAssetManager _instance = new AssetManager();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Guid, string> _idToPathMapping = new Dictionary<Guid, string>();

        private ContentManager _contentManager;

        internal AssetManager() {
        }

        /// <summary>
        /// Gets the singleton instance of an asset manager.
        /// </summary>
        public static IAssetManager Instance {
            get {
                return AssetManager._instance;
            }

            set {
                if (value != null) {
                    AssetManager._instance = value;
                }
            }
        }

        /// <inheritdoc/>
        public void ClearMappings() {
            this._idToPathMapping.Clear();
        }

        /// <inheritdoc/>
        public string GetPath(Guid id) {
            this._idToPathMapping.TryGetValue(id, out var result);
            return result;
        }

        /// <inheritdoc/>
        public void Initialize(ContentManager contentManager) {
            this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            AssetManager.Instance = this;
        }

        /// <inheritdoc/>
        public T Load<T>(string path) {
            T result;

            if (this._contentManager != null) {
                result = this._contentManager.Load<T>(path);
            }
            else {
                result = default;
            }

            return result;
        }

        /// <inheritdoc/>
        public T Load<T>(Guid id) {
            T result;

            if (this._contentManager != null && this._idToPathMapping.TryGetValue(id, out var path)) {
                result = this._contentManager.Load<T>(path);
            }
            else {
                result = default;
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetMapping(Guid id, string contentPath) {
            this._idToPathMapping[id] = contentPath;
        }

        /// <inheritdoc/>
        public void Unload() {
            this._contentManager?.Unload();
        }

        /// <inheritdoc/>
        public void Unload(Guid id) {
            if (this._contentManager != null && this._idToPathMapping.TryGetValue(id, out var path)) {
                // TODO: address the fact that this isn't possible this._contentManager.Unload(path);
            }
        }

        /// <inheritdoc/>
        public void Unload(string path) {
            if (this._contentManager != null) {
                // TODO: address the fact that this isn't possible this._contentManager.Unload(path);
            }
        }
    }
}
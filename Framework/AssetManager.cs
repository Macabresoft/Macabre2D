namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;
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
    }

    /// <summary>
    /// Maps content with identifiers. This should be the primary way that content is accessed.
    /// </summary>
    [DataContract]
    public sealed class AssetManager : IAssetManager {
        internal const string ContentFileName = "AssetManager";

        [DataMember]
        private readonly Dictionary<Guid, string> _idToStringMapping = new Dictionary<Guid, string>();

        private ContentManager _contentManager;

        internal AssetManager() {
        }

        /// <summary>
        /// Gets the singleton instance of an asset manager.
        /// </summary>
        public static IAssetManager Instance { get; internal set; }

        /// <inheritdoc/>
        public void ClearMappings() {
            this._idToStringMapping.Clear();
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

            if (this._contentManager != null && _idToStringMapping.TryGetValue(id, out var path)) {
                result = this._contentManager.Load<T>(path);
            }
            else {
                result = default;
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetMapping(Guid id, string contentPath) {
            this._idToStringMapping[id] = contentPath;
        }
    }
}
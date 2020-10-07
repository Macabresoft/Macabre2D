namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        /// Gets the identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The identifier associated with the path provided.</returns>
        Guid GetId(string path);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void Initialize(ContentManager contentManager);

        /// <summary>
        /// Sets the mapping.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="contentPath">The content path.</param>
        void SetMapping(Guid id, string contentPath);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="path">The path.</param>
        /// <returns>A value indicating whether or not the path was found.</returns>
        bool TryGetPath(Guid id, out string? path);

        /// <summary>
        /// Loads the asset at the specified path.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>The asset.</returns>
        bool TryLoad<T>(string path, out T? loaded) where T : class;

        /// <summary>
        /// Loads the asset with the specified identifier.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="Id">The identifier.</param>
        /// <returns>The asset.</returns>
        bool TryLoad<T>(Guid id, out T? loaded) where T : class;

        /// <summary>
        /// Unloads the content manager.
        /// </summary>
        void Unload();
    }

    /// <summary>
    /// Maps content with identifiers. This should be the primary way that content is accessed.
    /// </summary>
    [DataContract]
    public sealed class AssetManager : IAssetManager {

        /// <summary>
        /// The content file name for <see cref="AssetManager" />.
        /// </summary>
        public const string ContentFileName = "AssetManager";

        private static IAssetManager _instance = new AssetManager();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Guid, string> _idToPathMapping = new Dictionary<Guid, string>();

        private ContentManager? _contentManager;

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

        /// <inheritdoc />
        public void ClearMappings() {
            this._idToPathMapping.Clear();
        }

        /// <inheritdoc />
        public Guid GetId(string path) {
            return this._idToPathMapping.FirstOrDefault(x => x.Value == path).Key;
        }

        /// <inheritdoc />
        public void Initialize(ContentManager contentManager) {
            this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            AssetManager.Instance = this;
        }

        /// <inheritdoc />
        public void SetMapping(Guid id, string contentPath) {
            this._idToPathMapping[id] = contentPath;
        }

        /// <inheritdoc />
        public bool TryGetPath(Guid id, out string? path) {
            return this._idToPathMapping.TryGetValue(id, out path);
        }

        /// <inheritdoc />
        public bool TryLoad<T>(string path, out T? loaded) where T : class {
            if (this._contentManager != null) {
                loaded = this._contentManager.Load<T>(path);
            }
            else {
                loaded = null;
            }

            return loaded != null;
        }

        /// <inheritdoc />
        public bool TryLoad<T>(Guid id, out T? loaded) where T : class {
            if (this._contentManager != null && this._idToPathMapping.TryGetValue(id, out var path)) {
                try {
                    loaded = this._contentManager.Load<T>(path);
                }
                catch {
                    loaded = null;
                }
            }
            else {
                loaded = null;
            }

            return loaded != null;
        }

        /// <inheritdoc />
        public void Unload() {
            this._contentManager?.Unload();
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Content;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface to manage assets.
    /// </summary>
    public interface IAssetManager {
        /// <summary>
        /// Adds an asset.
        /// </summary>
        /// <param name="asset">The asset to add.</param>
        void AddAsset(IAsset asset);

        /// <summary>
        /// Clears the mappings.
        /// </summary>
        void ClearMappings();

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The identifier associated with the path provided.</returns>
        Guid GetContentId(string path);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void Initialize(ContentManager contentManager);

        /// <summary>
        /// Removes an asset.
        /// </summary>
        /// <param name="asset">The asset to remove.</param>
        /// <returns>A value indicating whether or not the asset was removed.</returns>
        bool RemoveAsset(IAsset asset);

        /// <summary>
        /// Resolves an asset reference and loads required content.
        /// </summary>
        /// <param name="assetReference">The asset reference to resolve.</param>
        /// <typeparam name="TAsset">The type of asset.</typeparam>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <returns>A value indicating whether or not the asset was resolved.</returns>
        bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference) where TAsset : class, IAsset where TContent : class;

        /// <summary>
        /// Sets the mapping.
        /// </summary>
        /// <param name="contentId">The identifier.</param>
        /// <param name="contentPath">The content path.</param>
        void SetContentMapping(Guid contentId, string contentPath);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="contentId">The identifier.</param>
        /// <param name="path">The path.</param>
        /// <returns>A value indicating whether or not the path was found.</returns>
        bool TryGetContentPath(Guid contentId, out string? path);

        /// <summary>
        /// Loads the asset at the specified path.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="contentPath">The path.</param>
        /// <param name="loaded">The loaded content.</param>
        /// <returns>The asset.</returns>
        bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class;

        /// <summary>
        /// Loads the asset with the specified identifier.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="loaded">The loaded content.</param>
        /// <returns>The asset.</returns>
        bool TryLoadContent<T>(Guid id, out T? loaded) where T : class;

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

        [DataMember]
        private readonly List<IAsset> _assets = new();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Guid, string> _idToPathMapping = new();

        private ContentManager? _contentManager;

        /// <summary>
        /// Gets the singleton instance of an asset manager.
        /// </summary>
        public static IAssetManager Instance { get; set; } = new AssetManager();

        /// <inheritdoc />
        public void AddAsset(IAsset asset) {
            if (this._assets.All(x => x.AssetId != asset.AssetId)) {
                this._assets.Add(asset);
            }
        }

        /// <inheritdoc />
        public void ClearMappings() {
            this._idToPathMapping.Clear();
        }

        /// <inheritdoc />
        public Guid GetContentId(string path) {
            return this._idToPathMapping.FirstOrDefault(x => x.Value == path).Key;
        }

        /// <inheritdoc />
        public void Initialize(ContentManager contentManager) {
            this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            Instance = this;

            foreach (var package in this._assets.OfType<IAssetPackage>()) {
                package.Initialize();
            }
        }

        /// <inheritdoc />
        public bool RemoveAsset(IAsset asset) {
            return this._assets.Remove(asset);
        }

        /// <inheritdoc />
        public bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference) where TAsset : class, IAsset where TContent : class {
            var result = false;

            if (assetReference.AssetId != Guid.Empty) {
                if (this._assets.OfType<TAsset>().FirstOrDefault(x => x.AssetId == assetReference.AssetId) is TAsset asset) {
                    if (asset is IContentAsset<TContent> contentAsset &&
                        contentAsset.ContentId != Guid.Empty &&
                        this.TryLoadContent<TContent>(contentAsset.ContentId, out var content) &&
                        content != null) {
                        contentAsset.LoadContent(content);
                    }
                    else if (asset is IAssetPackage<TAsset, TContent> {Content: null} package &&
                             this.TryLoadContent<TContent>(package.ContentId, out var packageContent) && 
                             packageContent != null) {
                        package.LoadContent(packageContent);
                    }

                    assetReference.Initialize(asset);
                    result = true;
                }
                else {
                    foreach (var package in this._assets.OfType<IAssetPackage<TAsset, TContent>>()) {
                        if (package.TryGetAsset(assetReference.AssetId, out var packagedAsset) && packagedAsset != null) {
                            if (package.Content == null && this.TryLoadContent<TContent>(package.ContentId, out var content) && content != null) {
                                package.LoadContent(content);
                            }

                            assetReference.Initialize(packagedAsset);
                        }
                    }
                }
            }
            
            return result;
        }

        /// <inheritdoc />
        public void SetContentMapping(Guid contentId, string contentPath) {
            this._idToPathMapping[contentId] = contentPath;
        }

        /// <inheritdoc />
        public bool TryGetContentPath(Guid contentId, out string? path) {
            return this._idToPathMapping.TryGetValue(contentId, out path);
        }

        /// <inheritdoc />
        public bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class {
            loaded = null;

            if (this._contentManager != null) {
                try {
                    loaded = this._contentManager.Load<T?>(contentPath);
                }
                catch (ContentLoadException) {
                }
            }

            return loaded != null;
        }

        /// <inheritdoc />
        public bool TryLoadContent<T>(Guid id, out T? loaded) where T : class {
            if (this._contentManager != null && this._idToPathMapping.TryGetValue(id, out var path)) {
                try {
                    loaded = this._contentManager.Load<T?>(path);
                }
                catch(Exception e) {
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
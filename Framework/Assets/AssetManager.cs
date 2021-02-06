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
        /// Resolves a packaged asset reference and loads required content.
        /// </summary>
        /// <param name="assetReference">The asset reference to resolve.</param>
        /// <typeparam name="TPackagedAsset">The type of asset being requested from an asset package.</typeparam>
        /// <typeparam name="TAssetPackage">The type of package the asset belongs to.</typeparam>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <returns>A value indicating whether or not the asset was resolved.</returns>
        bool ResolveAsset<TPackagedAsset, TAssetPackage, TContent>(PackagedAssetReference<TPackagedAsset, TAssetPackage> assetReference)
            where TAssetPackage : class, IAsset, IAssetPackage<TPackagedAsset>
            where TPackagedAsset : class, IAsset, IPackagedAsset<TAssetPackage>
            where TContent : class;

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
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Guid, string> _contentIdToPathMapping = new();

        [DataMember]
        private readonly List<IAsset> _loadedAssets = new();

        private ContentManager? _contentManager;

        /// <inheritdoc />
        public void AddAsset(IAsset asset) {
            if (this._loadedAssets.All(x => x.AssetId != asset.AssetId)) {
                this._loadedAssets.Add(asset);
            }
        }

        /// <inheritdoc />
        public void ClearMappings() {
            this._contentIdToPathMapping.Clear();
        }

        /// <inheritdoc />
        public Guid GetContentId(string path) {
            return this._contentIdToPathMapping.FirstOrDefault(x => x.Value == path).Key;
        }

        /// <inheritdoc />
        public void Initialize(ContentManager contentManager) {
            this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));

            foreach (var package in this._loadedAssets.OfType<IAssetPackage>()) {
                package.Initialize();
            }
        }

        /// <inheritdoc />
        public bool RemoveAsset(IAsset asset) {
            return this._loadedAssets.Remove(asset);
        }

        /// <inheritdoc />
        public bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference)
            where TAsset : class, IAsset
            where TContent : class {
            var asset = assetReference.Asset ?? this.GetAsset<TAsset>(assetReference.AssetId);
            if (asset != null) {
                this.LoadContentForAsset<TContent>(asset);
                assetReference.Initialize(asset);
            }

            return asset != null;
        }

        /// <inheritdoc />
        public bool ResolveAsset<TPackagedAsset, TAssetPackage, TContent>(PackagedAssetReference<TPackagedAsset, TAssetPackage> assetReference)
            where TAssetPackage : class, IAsset, IAssetPackage<TPackagedAsset>
            where TPackagedAsset : class, IAsset, IPackagedAsset<TAssetPackage>
            where TContent : class {
            var result = false;

            var package = this.GetAsset<TAssetPackage>(assetReference.PackageId);
            if (package != null && package.TryGetAsset(assetReference.AssetId, out var packagedAsset) && packagedAsset != null) {
                this.LoadContentForAsset<TContent>(package);
                assetReference.Initialize(packagedAsset);
                result = true;
            }

            return result;
        }

        /// <inheritdoc />
        public void SetContentMapping(Guid contentId, string contentPath) {
            this._contentIdToPathMapping[contentId] = contentPath;
        }

        /// <inheritdoc />
        public bool TryGetContentPath(Guid contentId, out string? path) {
            return this._contentIdToPathMapping.TryGetValue(contentId, out path);
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
            if (this._contentManager != null && this._contentIdToPathMapping.TryGetValue(id, out var path)) {
                try {
                    loaded = this._contentManager.Load<T?>(path);
                }
                catch (Exception) {
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

        private TAsset? GetAsset<TAsset>(Guid assetId) where TAsset : class, IAsset {
            TAsset? result = null;
            if (assetId != Guid.Empty) {
                if (this._loadedAssets.OfType<TAsset>().FirstOrDefault(x => x.AssetId == assetId) is TAsset asset) {
                    result = asset;
                }
            }

            return result;
        }

        private void LoadContentForAsset<TContent>(IAsset asset) where TContent : class {
            if (asset is IContentAsset<TContent> {Content: null} contentAsset && this.TryLoadContent<TContent>(contentAsset.ContentId, out var content) && content != null) {
                contentAsset.LoadContent(content);
            }
        }
    }
}
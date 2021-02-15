namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Content;
    using Newtonsoft.Json;

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
        Guid GetContentId(string path);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void Initialize(ContentManager contentManager);

        /// <summary>
        /// Loads the content meta data into the cache.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        void LoadMetadata(ContentMetadata metadata);

        /// <summary>
        /// Resets content mappings by clearing out the current mappings and replacing them with the newly provided mappings.
        /// </summary>
        /// <param name="contentIdToPathMapping">A map of content identifiers to their path.</param>
        void ResetContentMappings(IEnumerable<(Guid ContentId, string ContentPath)> contentIdToPathMapping);

        /// <summary>
        /// Resolves an asset reference and loads required content.
        /// </summary>
        /// <param name="assetReference">The asset reference to resolve.</param>
        /// <typeparam name="TAsset">The type of asset.</typeparam>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <returns>A value indicating whether or not the asset was resolved.</returns>
        bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference) where TAsset : class, IAsset where TContent : class;
        
        /// <summary>
        /// Sets the content mapping.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
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

        private readonly HashSet<ContentMetadata> _loadedMetadata = new();
        private ContentManager? _contentManager;

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
        }

        /// <inheritdoc />
        public void LoadMetadata(ContentMetadata metadata) {
            this._loadedMetadata.Add(metadata);
        }
        
        /// <inheritdoc />
        public void ResetContentMappings(IEnumerable<(Guid ContentId, string ContentPath)> contentIdToPathMapping) {
            this._contentIdToPathMapping.Clear();

            foreach (var (contentId, contentPath) in contentIdToPathMapping) {
                this.SetContentMapping(contentId, contentPath);
            }
        }

        /// <inheritdoc />
        public bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference)
            where TAsset : class, IAsset
            where TContent : class {
            var asset = assetReference.Asset ?? this.GetAsset<TAsset>(assetReference.ContentId);
            if (asset != null) {
                this.LoadContentForAsset<TContent>(asset);
                assetReference.Initialize(asset);
            }

            return asset != null;
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

        private TAsset? GetAsset<TAsset>(Guid contentId) where TAsset : class, IAsset {
            TAsset? result = null;
            if (contentId != Guid.Empty) {
                if (this._loadedMetadata.FirstOrDefault(x => x.ContentId == contentId)?.Asset is TAsset asset) {
                    result = asset;
                }
                else if (this.TryGetContentPath(contentId, out var path) &&
                         !string.IsNullOrWhiteSpace(path)) {
                    var metadataPath = Path.Combine(path, ContentMetadata.FileExtension);
                    if (this.TryLoadContent<ContentMetadata>(metadataPath, out var metadata) && metadata != null) {
                        this._loadedMetadata.Add(metadata);
                        result = metadata.Asset as TAsset;
                    }
                }
            }

            return result;
        }


        private void LoadContentForAsset<TContent>(IAsset asset) where TContent : class {
            if (asset is IAsset<TContent> {Content: null} contentAsset && 
                this.TryLoadContent<TContent>(asset.ContentId, out var content) && 
                content != null) {
                contentAsset.LoadContent(content);
            }
        }
    }
}
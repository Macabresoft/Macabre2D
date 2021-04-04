namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Interface to manage assets.
    /// </summary>
    public interface IAssetManager {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="serializer">The serializer.</param>
        void Initialize(ContentManager contentManager, ISerializer serializer);

        /// <summary>
        /// Registers the content meta data into the cache.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        void RegisterMetadata(ContentMetadata metadata);

        /// <summary>
        /// Resolves an asset reference and loads required content.
        /// </summary>
        /// <param name="assetReference">The asset reference to resolve.</param>
        /// <typeparam name="TAsset">The type of asset.</typeparam>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <returns>A value indicating whether or not the asset was resolved.</returns>
        bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference) where TAsset : class, IAsset where TContent : class;

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
    public sealed class AssetManager : IAssetManager {
        private const string MonoGameNamespace = nameof(Microsoft) + "." + nameof(Microsoft.Xna) + "." + nameof(Microsoft.Xna.Framework);
        private readonly HashSet<ContentMetadata> _loadedMetadata = new();
        private ContentManager? _contentManager;
        private ISerializer? _serializer;

        /// <summary>
        /// The default empty <see cref="IAssetManager" /> that is present before initialization.
        /// </summary>
        public static readonly IAssetManager Empty = new EmptyAssetManager();
        
        /// <inheritdoc />
        public void Initialize(ContentManager contentManager, ISerializer serializer) {
            this._contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <inheritdoc />
        public void RegisterMetadata(ContentMetadata metadata) {
            this._loadedMetadata.Add(metadata);
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
        public bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class {
            loaded = null;
            if (this._contentManager != null) {
                if (typeof(T).Namespace?.StartsWith(MonoGameNamespace) == true) {
                    try {
                        loaded = this._contentManager.Load<T?>(contentPath);
                    }
                    catch (ContentLoadException) {
                        // TODO: log this
                    }
                }
                else if (this._serializer != null) {
                    try {
                        var fileStream = TitleContainer.OpenStream(Path.Combine(this._contentManager.RootDirectory, contentPath));
                        loaded = this._serializer.Deserialize<T>(fileStream);
                    }
                    catch(FileNotFoundException) {
                        // TODO: log this
                    }
                }
            }
            
            return loaded != null;
        }

        /// <inheritdoc />
        public bool TryLoadContent<T>(Guid id, out T? loaded) where T : class {
            if (this.TryGetContentPath(id, out var path) && !string.IsNullOrWhiteSpace(path)) {
                this.TryLoadContent(path, out loaded);
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
                else if (this.TryGetContentMetadata(contentId, out var metadata) && metadata != null) {
                    result = metadata.Asset as TAsset;
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

        private bool TryGetContentMetadata(Guid contentId, out ContentMetadata? metadata) {
            if (this._loadedMetadata.FirstOrDefault(x => x.ContentId == contentId) is ContentMetadata cachedMetadata) {
                metadata = cachedMetadata;
            }
            else if (this.TryLoadContent(ContentMetadata.GetMetadataPath(contentId), out ContentMetadata? foundMetadata) && foundMetadata != null) {
                metadata = foundMetadata;
                this.RegisterMetadata(foundMetadata);
            }
            else {
                metadata = null;
            }

            return metadata != null;
        }

        private bool TryGetContentPath(Guid contentId, out string? contentPath) {
            if (this.TryGetContentMetadata(contentId, out var metadata) && metadata != null) {
                contentPath = metadata.GetContentPath();
            }
            else {
                contentPath = null;
            }

            return !string.IsNullOrEmpty(contentPath);
        }

        private sealed class EmptyAssetManager : IAssetManager {
            /// <inheritdoc />
            public void Initialize(ContentManager contentManager, ISerializer serializer) {
            }

            /// <inheritdoc />
            public void RegisterMetadata(ContentMetadata metadata) {
            }

            /// <inheritdoc />
            public bool ResolveAsset<TAsset, TContent>(AssetReference<TAsset> assetReference) where TAsset : class, IAsset where TContent : class {
                return false;
            }

            /// <inheritdoc />
            public bool TryLoadContent<T>(string contentPath, out T? loaded) where T : class {
                loaded = null;
                return false;
            }

            /// <inheritdoc />
            public bool TryLoadContent<T>(Guid id, out T? loaded) where T : class {
                loaded = null;
                return false;
            }

            /// <inheritdoc />
            public void Unload() {
            }
        }
    }
}
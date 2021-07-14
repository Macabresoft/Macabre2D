namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.ComponentModel;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which handles the <see cref="IScene" /> open in the editor.
    /// </summary>
    public interface ISceneService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        IScene CurrentScene { get; }

        /// <summary>
        /// Gets the current scene metadata.
        /// </summary>
        ContentMetadata CurrentSceneMetadata { get; }

        /// <summary>
        /// Saves the current scene.
        /// </summary>
        void SaveScene();

        /// <summary>
        /// Saves the provided scene and metadata.
        /// </summary>
        void SaveScene(ContentMetadata metadata, IScene scene);

        /// <summary>
        /// Tries to load a scene.
        /// </summary>
        /// <param name="contentId">The content identifier of the scene.</param>
        /// <param name="sceneAsset">The scene asset.</param>
        /// <returns>A value indicating whether or not the scene was loaded.</returns>
        bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset);
    }

    /// <summary>
    /// A service which handles the <see cref="IScene" /> open in the editor.
    /// </summary>
    public sealed class SceneService : ReactiveObject, ISceneService {
        private readonly IAssetManager _assetManager;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly ISerializer _serializer;
        private ContentMetadata _currentSceneMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneService" /> class.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="serializer">The serializer.</param>
        public SceneService(
            IAssetManager assetManager,
            IFileSystemService fileSystem,
            IPathService pathService,
            ISerializer serializer) {
            this._assetManager = assetManager;
            this._fileSystem = fileSystem;
            this._pathService = pathService;
            this._serializer = serializer;
        }

        /// <inheritdoc />
        public IScene CurrentScene => (this._currentSceneMetadata?.Asset as SceneAsset)?.Content;

        /// <inheritdoc />
        public ContentMetadata CurrentSceneMetadata {
            get => this._currentSceneMetadata;
            private set {
                this.RaiseAndSetIfChanged(ref this._currentSceneMetadata, value);
                this.RaisePropertyChanged(nameof(this.CurrentScene));
            }
        }

        /// <inheritdoc />
        public void SaveScene() {
            this.SaveScene(this.CurrentSceneMetadata, this.CurrentScene);
        }

        /// <inheritdoc />
        public void SaveScene(ContentMetadata metadata, IScene scene) {
            if (metadata != null && scene != null && metadata.Asset is IAsset<Scene>) {
                var metadataPath = this._pathService.GetMetadataFilePath(metadata.ContentId);
                this._serializer.Serialize(metadata, metadataPath);

                var filePath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());
                this._serializer.Serialize(scene, filePath);
            }
        }

        /// <inheritdoc />
        public bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset) {
            if (contentId == Guid.Empty) {
                sceneAsset = null;
            }
            else {
                var metadataFilePath = this._pathService.GetMetadataFilePath(contentId);
                if (this._fileSystem.DoesFileExist(metadataFilePath)) {
                    var metadata = this._serializer.Deserialize<ContentMetadata>(metadataFilePath);
                    sceneAsset = metadata?.Asset as SceneAsset;

                    if (metadata != null && sceneAsset != null) {
                        var contentPath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());

                        if (this._fileSystem.DoesFileExist(contentPath)) {
                            var scene = this._serializer.Deserialize<Scene>(contentPath);
                            if (scene != null) {
                                sceneAsset.LoadContent(scene);
                                this.CurrentSceneMetadata = metadata;
                            }
                        }
                    }
                }
                else {
                    sceneAsset = null;
                }
            }

            return sceneAsset != null;
        }
    }
}
namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using Assimp;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public interface ISceneService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        public IGameScene CurrentScene { get; }
        
        /// <summary>
        /// Gets or sets a value which indicates whether or not the scene has changes which require saving.
        /// </summary>
        bool HasChanges { get; set; }

        /// <summary>
        /// Creates the new scene and serializes it.
        /// </summary>
        /// <param name="parent">The parent content directory.</param>
        /// <param name="sceneName">The name of the scene without its file extension.</param>
        /// <returns>The newly created scene wrapped in a <see cref="SceneAsset" />.</returns>
        SceneAsset CreateNewScene(IContentDirectory parent, string sceneName);

        /// <summary>
        /// Tries to load a scene.
        /// </summary>
        /// <param name="contentId">The content identifier of the scene.</param>
        /// <param name="sceneAsset">The scene asset.</param>
        /// <returns>A value indicating whether or not the scene was loaded.</returns>
        bool TryLoadScene(Guid contentId, out SceneAsset sceneAsset);
    }

    /// <summary>
    /// A service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public sealed class SceneService : ReactiveObject, ISceneService {
        private readonly IAssetManager _assetManager;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly ISerializer _serializer;
        private Guid _currentSceneContentId;
        private IGameScene _currentScene;
        private bool _hasChanges;

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
        public IGameScene CurrentScene {
            get => this._currentScene;
        }
        
        /// <inheritdoc />
        public bool HasChanges {
            get => this._hasChanges;
            set => this.RaiseAndSetIfChanged(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        public SceneAsset CreateNewScene(IContentDirectory parent, string sceneName) {
            var contentDirectoryPath = parent.GetFullPath();
            if (!this._fileSystem.DoesDirectoryExist(contentDirectoryPath)) {
                throw new DirectoryNotFoundException();
            }

            GameScene scene;
            var filePath = Path.Combine(contentDirectoryPath, $"{sceneName}{SceneAsset.FileExtension}");
            if (this._fileSystem.DoesFileExist(filePath)) {
                // TODO: Overwrite warning.
                scene = this._serializer.Deserialize<GameScene>(filePath);
            }
            else {
                scene = new GameScene {
                    BackgroundColor = DefinedColors.MacabresoftPurple,
                    Name = sceneName
                };

                this._serializer.Serialize(scene, filePath);
            }

            var sceneAsset = new SceneAsset {
                Name = scene.Name
            };
            

            sceneAsset.LoadContent(scene);
            var contentPath = Path.Combine(parent.GetContentPath(), sceneName);
            var metadata = new ContentMetadata(
                sceneAsset,
                contentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList(),
                SceneAsset.FileExtension);

            var metadataPath = Path.Combine(contentDirectoryPath, ContentMetadata.GetMetadataPath(sceneAsset.ContentId));
            this._serializer.Serialize(metadata, metadataPath);
            this._assetManager.RegisterMetadata(metadata);

            var contentFile = new ContentFile(parent, metadata);

            this.SetCurrentScene(scene, contentFile.Asset.ContentId);
            return sceneAsset;
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

                    if (sceneAsset != null) {
                        // ReSharper disable once PossibleNullReferenceException
                        var contentPath = Path.Combine(this._pathService.ContentDirectoryPath, metadata.GetContentPath());

                        if (this._fileSystem.DoesFileExist(contentPath)) {
                            var scene = this._serializer.Deserialize<GameScene>(contentPath);
                            if (scene != null) {
                                sceneAsset.LoadContent(scene);
                                this.SetCurrentScene(scene, contentId);
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

        private void SetCurrentScene(IGameScene scene, Guid contentId) {
            this._currentSceneContentId = contentId;
            this.RaiseAndSetIfChanged(ref this._currentScene, scene);
        }

    }
}
namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public interface ISceneService : INotifyPropertyChanged {
        /// <summary>
        /// Gets or sets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        public IGameScene CurrentScene { get; set; }
        
        /// <summary>
        /// Gets or sets a value which indicates whether or not the scene has changes which require saving.
        /// </summary>
        bool HasChanges { get; set; }
        
        /// <summary>
        /// Creates the new scene and serializes it.
        /// </summary>
        /// <param name="contentDirectoryPath">The full path to the content directory.</param>
        /// <param name="contentPath">The content path of the new scene.</param>
        /// <returns>The newly created scene wrapped in a <see cref="SceneAsset" />.</returns>
        SceneAsset CreateNewScene(string contentDirectoryPath, string contentPath);
    }

    /// <summary>
    /// A service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public sealed class SceneService : ReactiveObject, ISceneService {
        private readonly IFileSystemService _fileSystem;
        private readonly ISerializer _serializer;
        private IGameScene _currentScene;
        private bool _hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneService" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="serializer">The serializer.</param>
        public SceneService(IFileSystemService fileSystem, ISerializer serializer) {
            this._fileSystem = fileSystem;
            this._serializer = serializer;
        }
        
        /// <inheritdoc />
        public IGameScene CurrentScene {
            get => this._currentScene;
            set => this.RaiseAndSetIfChanged(ref this._currentScene, value);
        }
        
        /// <inheritdoc />
        public bool HasChanges {
            get => this._hasChanges;
            set => this.RaiseAndSetIfChanged(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        public SceneAsset CreateNewScene(string contentDirectoryPath, string contentPath) {
            if (!this._fileSystem.DoesDirectoryExist(contentDirectoryPath)) {
                throw new DirectoryNotFoundException();
            }

            GameScene scene;
            var filePath = Path.Combine(contentDirectoryPath, $"{contentPath}{SceneAsset.FileExtension}");
            if (this._fileSystem.DoesFileExist(filePath)) {
                // TODO: Overwrite warning.
                scene = this._serializer.Deserialize<GameScene>(filePath);
            }
            else {
                scene = new GameScene {
                    BackgroundColor = DefinedColors.MacabresoftPurple,
                    Name = Path.GetFileName(contentPath)
                };

                this._serializer.Serialize(scene, filePath);
            }

            var sceneAsset = new SceneAsset {
                Name = scene.Name
            };

            sceneAsset.LoadContent(scene);
            var metadata = new ContentMetadata(
                sceneAsset,
                contentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList(),
                SceneAsset.FileExtension);

            var metadataPath = Path.Combine(contentDirectoryPath, ContentMetadata.GetMetadataPath(sceneAsset.ContentId));
            this._serializer.Serialize(metadata, metadataPath);

            // TODO: Maybe check if the content service is initialized and add this? Maybe just add it blindly and have the content service just ig

            return sceneAsset;
        }
    }
}
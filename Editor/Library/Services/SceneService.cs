namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.ComponentModel;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public interface ISceneService : INotifyPropertyChanged {

        /// <summary>
        /// Creates the new scene and serializes it.
        /// </summary>
        /// <typeparam name="T">The type of scene.</typeparam>
        /// <param name="parentDirectoryPath">The scene content file's parent directory path.</param>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The newly created scene.</returns>
        T CreateNewScene<T>(string parentDirectoryPath, string sceneName) where T : IGameScene, new();
    }

    /// <summary>
    /// A service which handles the <see cref="IGameScene" /> open in the editor.
    /// </summary>
    public sealed class SceneService : ReactiveObject, ISceneService {
        private readonly IFileSystemService _fileSystem;
        private readonly ISerializer _serializer;

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
        public T CreateNewScene<T>(string parentDirectoryPath, string sceneName) where T : IGameScene, new() {
            if (!this._fileSystem.DoesDirectoryExist(parentDirectoryPath)) {
                throw new DirectoryNotFoundException();
            }

            T scene;
            var filePath = Path.Combine(parentDirectoryPath, $"{sceneName}{SceneAsset.FileExtension}");
            if (this._fileSystem.DoesFileExist(filePath)) {
                // TODO: Override warning.
                scene = this._serializer.Deserialize<T>(filePath);
            }
            else {
                scene = new T {
                    BackgroundColor = DefinedColors.MacabresoftPurple,
                    Name = sceneName
                };

                this._serializer.Serialize(scene, filePath);
            }
            
            // TODO: Maybe check if the content service is initialized and add this? Maybe just add it blindly and have the content service just ig

            return scene;
        }
    }
}
﻿namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
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
        /// <param name="contentDirectoryPath">The full path to the content directory.</param>
        /// <param name="contentPath">The content path of the new scene.</param>
        /// <returns>The newly created scene.</returns>
        GameScene CreateNewScene(string contentDirectoryPath, string contentPath);
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
        public GameScene CreateNewScene(string contentDirectoryPath, string contentPath) {
            if (!this._fileSystem.DoesDirectoryExist(contentDirectoryPath)) {
                throw new DirectoryNotFoundException();
            }

            GameScene scene;
            var filePath = Path.Combine(contentDirectoryPath, $"{contentPath}{SceneAsset.FileExtension}");
            if (this._fileSystem.DoesFileExist(filePath)) {
                // TODO: Override warning.
                scene = this._serializer.Deserialize<GameScene>(filePath);
            }
            else {
                scene = new GameScene {
                    BackgroundColor = DefinedColors.MacabresoftPurple,
                    Name = Path.GetFileName(contentPath)
                };

                this._serializer.Serialize(scene, filePath);
            }

            var sceneAsset = new SceneAsset();
            sceneAsset.LoadContent(scene);
            var metadata = new ContentMetadata(
                sceneAsset,
                contentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries),
                SceneAsset.FileExtension);

            var metadataPath = Path.Combine(contentDirectoryPath, ContentMetadata.GetMetadataPath(sceneAsset.ContentId));
            this._serializer.Serialize(metadata, metadataPath);

            // TODO: Maybe check if the content service is initialized and add this? Maybe just add it blindly and have the content service just ig

            return scene;
        }
    }
}
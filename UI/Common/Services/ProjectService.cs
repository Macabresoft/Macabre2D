namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which loads, saves, and exposes a <see cref="GameProject" />.
    /// </summary>
    public interface IProjectService : INotifyPropertyChanged {
        /// <summary>
        /// Gets the currently loaded project.
        /// </summary>
        GameProject CurrentProject { get; }

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <returns>The loaded project.</returns>
        GameProject LoadProject();

        /// <summary>
        /// Saves the currently opened project.
        /// </summary>
        void SaveProject();
    }

    /// <summary>
    /// A service which loads, saves, and exposes a <see cref="GameProject" />.
    /// </summary>
    public sealed class ProjectService : ReactiveObject, IProjectService {
        private readonly IContentService _contentService;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly ISceneService _sceneService;
        private readonly ISerializer _serializer;
        private GameProject _currentProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="serializer">The serializer.</param>
        public ProjectService(
            IContentService contentService,
            IFileSystemService fileSystem,
            IPathService pathService,
            ISceneService sceneService,
            ISerializer serializer) {
            this._contentService = contentService;
            this._fileSystem = fileSystem;
            this._pathService = pathService;
            this._sceneService = sceneService;
            this._serializer = serializer;
        }

        /// <inheritdoc />
        public GameProject CurrentProject {
            get => this._currentProject;
            private set => this.RaiseAndSetIfChanged(ref this._currentProject, value);
        }

        /// <inheritdoc />
        public GameProject LoadProject() {
            this._contentService.RefreshContent();

            var projectExists = this._fileSystem.DoesFileExist(this._pathService.ProjectFilePath);
            if (!projectExists) {
                this._fileSystem.CreateDirectory(this._pathService.ContentDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.MetadataArchiveDirectoryPath);
                this._fileSystem.CreateDirectory(this._pathService.MetadataDirectoryPath);

                this.CurrentProject = new GameProject {
                    StartupSceneContentId = this.CreateInitialScene()
                };

                this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
            }
            else {
                this.CurrentProject = this._serializer.Deserialize<GameProject>(this._pathService.ProjectFilePath);
                if (!this._sceneService.TryLoadScene(this.CurrentProject.StartupSceneContentId, out var sceneAsset) && sceneAsset != null) {
                    this.CurrentProject.StartupSceneContentId = this.CreateInitialScene();
                }
            }

            return this.CurrentProject;
        }

        /// <inheritdoc />
        public void SaveProject() {
            this.SaveProjectFile(this.CurrentProject, this._pathService.ProjectFilePath);
        }

        private Guid CreateInitialScene() {
            var newSceneAsset = this._sceneService.CreateNewScene(this._contentService.RootContentDirectory, "Default Scene");
            return newSceneAsset.ContentId;
        }

        private void SaveProjectFile(IGameProject project, string projectFilePath) {
            if (project != null && !string.IsNullOrWhiteSpace(projectFilePath)) {
                this._serializer.Serialize(project, projectFilePath);
            }
        }
    }
}
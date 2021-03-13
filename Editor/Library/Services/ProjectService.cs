namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// Interface for a service which loads, saves, and exposes a <see cref="GameProject" />.
    /// </summary>
    public interface IProjectService {
        /// <summary>
        /// Gets the currently loaded project.
        /// </summary>
        IGameProject CurrentProject { get; }

        /// <summary>
        /// Gets or sets a value which indicates whether or not the project has changes which require saving.
        /// </summary>
        bool HasChanges { get; set; }

        /// <summary>
        /// Creates a project at the specified path.
        /// </summary>
        /// <param name="projectDirectoryPath">Path to the directory where the project should be created.</param>
        /// <returns>The created project.</returns>
        IGameProject CreateProject(string projectDirectoryPath);

        /// <summary>
        /// Loads the project at the specified path.
        /// </summary>
        /// <param name="projectFilePath">The project file path.</param>
        /// <returns>The loaded project.</returns>
        IGameProject LoadProject(string projectFilePath);

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
        private readonly ISceneService _sceneService;
        private readonly ISerializer _serializer;
        private IGameProject _currentProject;
        private bool _hasChanges;
        private string _projectFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="serializer">The serializer.</param>
        public ProjectService(
            IContentService contentService,
            IFileSystemService fileSystem,
            ISceneService sceneService,
            ISerializer serializer) : base() {
            this._contentService = contentService;
            this._fileSystem = fileSystem;
            this._sceneService = sceneService;
            this._serializer = serializer;
        }

        /// <inheritdoc />
        public IGameProject CurrentProject {
            get => this._currentProject;
            private set => this.RaiseAndSetIfChanged(ref this._currentProject, value);
        }

        /// <inheritdoc />
        public bool HasChanges {
            get => this._hasChanges;
            set => this.RaiseAndSetIfChanged(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        public IGameProject CreateProject(string projectDirectoryPath) {
            var projectFilePath = Path.Combine(projectDirectoryPath, GameProject.ProjectFileName);

            if (this._fileSystem.DoesFileExist(projectFilePath)) {
                throw new NotSupportedException();
            }

            var project = new GameProject();
            this._sceneService.CreateNewScene<GameScene>(Path.Combine(projectDirectoryPath, GameProject.ContentDirectoryName), "Default Scene");

            // TODO: create scene, save it, and place it in the content hierarchy
            /*var startupScene = new GameScene();
            var sceneAsset = new SceneAsset();
            sceneAsset.LoadContent(startupScene);
            this.CurrentProject.Assets.*/

            this.SaveProjectFile(project, projectFilePath);
            return this.LoadProject(projectFilePath);
        }

        /// <inheritdoc />
        public IGameProject LoadProject(string projectFilePath) {
            this._projectFilePath = this._fileSystem.DoesFileExist(projectFilePath) ? projectFilePath : throw new NotSupportedException();
            this.CurrentProject = this._serializer.Deserialize<GameProject>(projectFilePath);
            this._contentService.Initialize(Path.GetDirectoryName(projectFilePath), this.CurrentProject.Assets);
            return this.CurrentProject;
        }

        /// <inheritdoc />
        public void SaveProject() {
            this.SaveProjectFile(this.CurrentProject, this._projectFilePath);
        }

        private void SaveProjectFile(IGameProject project, string projectFilePath) {
            if (project != null && !string.IsNullOrWhiteSpace(projectFilePath)) {
                this._serializer.Serialize(project, projectFilePath);
            }
        }
    }
}
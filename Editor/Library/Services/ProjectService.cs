namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.IO;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
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
        private readonly ISerializer _serializer;
        private IGameProject _currentProject;
        private bool _hasChanges;
        private string _projectFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        public ProjectService(ISerializer serializer) : base() {
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
        public IGameProject LoadProject(string projectFilePath) {
            this._projectFilePath = projectFilePath;
            var project = File.Exists(projectFilePath) ? this._serializer.Deserialize<GameProject>(projectFilePath) : CreateGameProject();
            return project;
        }

        /// <inheritdoc />
        public void SaveProject() {
            if (this.CurrentProject != null && !string.IsNullOrWhiteSpace(this._projectFilePath)) {
                this._serializer.Serialize(this.CurrentProject, this._projectFilePath);
            }
        }

        private static IGameProject CreateGameProject() {
            return new GameProject();
        }
    }
}
namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly EditorGame _editorGame;
        private readonly IProjectService _projectService;
        private readonly ISceneService _sceneService;
        private bool _showGrid = true;
        private bool _showSelection = true;

        public MonoGameService(EditorGame editorGame, IProjectService projectService, ISceneService sceneService) {
            this._editorGame = editorGame;
            this._sceneService = sceneService;
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
            this._editorGame.CurrentScene = this._sceneService.CurrentScene?.Scene;
            this._projectService = projectService;
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._editorGame.GameSettings = this._projectService.CurrentProject?.GameSettings;
            this.SetContentPath();
        }

        public DependencyObject EditorGame {
            get {
                return this._editorGame;
            }
        }

        public bool ShowGrid {
            get {
                return this._showGrid;
            }

            set {
                this.Set(ref this._showGrid, value);
                this._editorGame.ShowGrid = this._showGrid;
            }
        }

        public bool ShowSelection {
            get {
                return this._showSelection;
            }

            set {
                this.Set(ref this._showSelection, value);
                this._editorGame.ShowSelection = this._showSelection;
            }
        }

        public void ResetCamera() {
            this._editorGame.ResetCamera();
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._projectService.CurrentProject)) {
                this._editorGame.GameSettings = this._projectService.CurrentProject?.GameSettings;
                this.SetContentPath();
            }
        }

        private void SceneService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._sceneService.CurrentScene)) {
                this._editorGame.CurrentScene = this._sceneService.CurrentScene?.Scene;
            }
        }

        private void SetContentPath() {
            if (this._projectService.CurrentProject != null) {
                var configurationName = "Debug"; //TODO : allow release
                var desktopBuildConfiguration = this._projectService.CurrentProject.BuildConfigurations.FirstOrDefault(x => x.Platform == BuildPlatform.DesktopGL);
                this._editorGame.SetContentPath(desktopBuildConfiguration.GetCompiledContentPath(this._projectService.GetSourcePath(), configurationName));
            }
        }
    }
}
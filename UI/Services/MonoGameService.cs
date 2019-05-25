namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly EditorGame _editorGame;
        private readonly IProjectService _projectService;
        private readonly ISceneService _sceneService;
        private bool _showGrid = true;
        private bool _showRotationGizmo;
        private bool _showScaleGizmo;
        private bool _showSelection = true;
        private bool _showTranslationGizmo = true;

        public MonoGameService(EditorGame editorGame, IProjectService projectService, ISceneService sceneService) {
            this._editorGame = editorGame;
            this._sceneService = sceneService;
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
            this._editorGame.CurrentScene = this._sceneService.CurrentScene?.Scene;
            this._projectService = projectService;
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._editorGame.AssetManager = this._projectService.CurrentProject?.AssetManager;
            this._editorGame.Settings = this._projectService.CurrentProject?.GameSettings;
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

        public bool ShowRotationGizmo {
            get {
                return this._showRotationGizmo;
            }

            set {
                if (this._showRotationGizmo && !this._showScaleGizmo && !this._showTranslationGizmo && !value) {
                    return;
                }

                this.Set(ref this._showRotationGizmo, value);
                this._editorGame.ShowRotationGizmo = this._showRotationGizmo;

                if (this._showRotationGizmo) {
                    this.ShowScaleGizmo = false;
                    this.ShowTranslationGizmo = false;
                }
            }
        }

        public bool ShowScaleGizmo {
            get {
                return this._showScaleGizmo;
            }

            set {
                if (this._showScaleGizmo && !this._showRotationGizmo && !this._showTranslationGizmo && !value) {
                    return;
                }

                this.Set(ref this._showScaleGizmo, value);
                this._editorGame.ShowScaleGizmo = this._showScaleGizmo;

                if (this._showScaleGizmo) {
                    this.ShowRotationGizmo = false;
                    this.ShowTranslationGizmo = false;
                }
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

        public bool ShowTranslationGizmo {
            get {
                return this._showTranslationGizmo;
            }

            set {
                if (this._showTranslationGizmo && !this._showRotationGizmo && !this._showScaleGizmo && !value) {
                    return;
                }

                this.Set(ref this._showTranslationGizmo, value);
                this._editorGame.ShowTranslationGizmo = this._showTranslationGizmo;

                if (this._showTranslationGizmo) {
                    this.ShowRotationGizmo = false;
                    this.ShowScaleGizmo = false;
                }
            }
        }

        public void CenterCamera() {
            if (this._editorGame.CurrentCamera != null) {
                this._editorGame.CurrentCamera.LocalPosition = Vector2.Zero;
            }
        }

        public void ResetCamera() {
            this._editorGame.ResetCamera();
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._projectService.CurrentProject)) {
                this._editorGame.AssetManager = this._projectService.CurrentProject?.AssetManager;
                this._editorGame.Settings = this._projectService.CurrentProject?.GameSettings;
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
                var desktopBuildConfiguration = this._projectService.CurrentProject.BuildConfigurations.FirstOrDefault(x => x.Platform == BuildPlatform.DesktopGL);
                this._editorGame.SetContentPath(desktopBuildConfiguration.GetCompiledContentPath(this._projectService.GetSourcePath(), BuildMode.Debug));
            }
        }
    }
}
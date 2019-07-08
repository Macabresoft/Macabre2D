namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly IComponentService _componentService;
        private readonly EditorGame _editorGame;

        private readonly Dictionary<GizmoType, ComponentEditingStyle> _gizmoTypeToMatchingEditingStyles = new Dictionary<GizmoType, ComponentEditingStyle> {
            { GizmoType.None, ~ComponentEditingStyle.None },
            { GizmoType.Translation, ComponentEditingStyle.Translation },
            { GizmoType.Scale, ComponentEditingStyle.Scale },
            { GizmoType.Rotation, ComponentEditingStyle.Rotation },
            { GizmoType.Tile, ComponentEditingStyle.Translation }
        };

        private readonly IProjectService _projectService;
        private readonly ISceneService _sceneService;
        private GizmoType _selectedGizmo = GizmoType.Translation;
        private bool _showGrid = true;
        private bool _showSelection = true;

        public MonoGameService(EditorGame editorGame, IComponentService componentService, IProjectService projectService, ISceneService sceneService) {
            this._editorGame = editorGame;
            this._componentService = componentService;
            this._componentService.SelectionChanged += this.ComponentService_SelectionChanged;
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

        public GizmoType SelectedGizmo {
            get {
                return this._selectedGizmo;
            }

            set {
                if (this.Set(ref this._selectedGizmo, value)) {
                    this._editorGame.SelectedGizmo = this.SelectedGizmo;
                }
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

        public void CenterCamera() {
            if (this._editorGame.CurrentCamera != null) {
                this._editorGame.CurrentCamera.LocalPosition = Vector2.Zero;
            }
        }

        public void ResetCamera() {
            this._editorGame.ResetCamera();
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (this.SelectedGizmo != GizmoType.None) {
                if (e.NewValue != null && this._gizmoTypeToMatchingEditingStyles.TryGetValue(this.SelectedGizmo, out var validEditingStyle)) {
                    if ((validEditingStyle & e.NewValue.EditingStyle) == ComponentEditingStyle.None) {
                        if ((e.NewValue.EditingStyle & ComponentEditingStyle.Translation) == ComponentEditingStyle.Translation) {
                            this.SelectedGizmo = GizmoType.Translation;
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Scale) == ComponentEditingStyle.Scale) {
                            this.SelectedGizmo = GizmoType.Scale;
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Rotation) == ComponentEditingStyle.Rotation) {
                            this.SelectedGizmo = GizmoType.Rotation;
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Tile) == ComponentEditingStyle.Tile) {
                            this.SelectedGizmo = GizmoType.Tile;
                        }
                        else {
                            this.SelectedGizmo = GizmoType.None;
                        }
                    }
                }
                else {
                    this.SelectedGizmo = GizmoType.None;
                }
            }
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
namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly IComponentService _componentService;
        private readonly EditorGame _editorGame;
        private readonly IFileService _fileService;
        private readonly IProjectService _projectService;

        private readonly ComponentEditingStyle[] _recentlyUsedEditingStyles = new[] {
            ComponentEditingStyle.Translation,
            ComponentEditingStyle.Scale,
            ComponentEditingStyle.Rotation,
            ComponentEditingStyle.Tile
        };

        private readonly ISceneService _sceneService;
        private ComponentEditingStyle _editingStyle = ComponentEditingStyle.Translation;
        private bool _hasSelectedGizmoBeenManuallyChanged = false;
        private bool _showGrid = true;
        private bool _showSelection = true;

        public MonoGameService(
            EditorGame editorGame,
            IComponentService componentService,
            IFileService fileService,
            IProjectService projectService,
            ISceneService sceneService) {
            this._editorGame = editorGame;
            this._componentService = componentService;
            this._componentService.SelectionChanged += this.ComponentService_SelectionChanged;
            this._fileService = fileService;
            this._sceneService = sceneService;
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
            this._editorGame.CurrentScene = this._sceneService.CurrentScene?.Scene;
            this._projectService = projectService;
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._editorGame.AssetManager = this._projectService.CurrentProject?.AssetManager;
            this._editorGame.Settings = this._projectService.CurrentProject?.GameSettings;
            this.SetContentPath();
        }

        public ComponentEditingStyle EditingStyle {
            get {
                return this._editingStyle;
            }

            set {
                if (this.SetEditingStyle(value)) {
                    this._hasSelectedGizmoBeenManuallyChanged = true;
                }
            }
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

        public void CenterCamera() {
            if (this._editorGame.CurrentCamera != null) {
                this._editorGame.CurrentCamera.LocalPosition = Vector2.Zero;
            }
        }

        public void ResetCamera() {
            this._editorGame.ResetCamera();
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            var editingStyleOverride = this.GetEditingStyleOverride(e.NewValue);

            if (e.NewValue != null && editingStyleOverride == ComponentEditingStyle.None && !this._hasSelectedGizmoBeenManuallyChanged && e.NewValue.EditingStyle != ComponentEditingStyle.None) {
                foreach (var editingStyle in this._recentlyUsedEditingStyles) {
                    if (e.NewValue.EditingStyle.HasFlag(editingStyle)) {
                        editingStyleOverride = editingStyle;
                        break;
                    }
                }
            }

            this.SetEditingStyle(editingStyleOverride);
        }

        private ComponentEditingStyle GetEditingStyleOverride(ComponentWrapper wrapper) {
            var result = ComponentEditingStyle.None;
            if (wrapper != null) {
                if (wrapper.EditingStyle.HasFlag(ComponentEditingStyle.Tile)) {
                    result = ComponentEditingStyle.Tile;
                }
                else if (wrapper.EditingStyle.HasFlag(this.EditingStyle)) {
                    result = this.EditingStyle;
                }
            }

            return result;
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
                this._editorGame.SetContentPath(desktopBuildConfiguration.GetCompiledContentPath(this._fileService.ProjectDirectoryPath, BuildMode.Debug));
            }
        }

        private bool SetEditingStyle(ComponentEditingStyle newEditingStyle) {
            this._hasSelectedGizmoBeenManuallyChanged = false;

            if (!((int)newEditingStyle).IsPowerOfTwo()) {
                newEditingStyle = Enum.GetValues(typeof(ComponentEditingStyle)).Cast<ComponentEditingStyle>().FirstOrDefault(x => x != ComponentEditingStyle.None && newEditingStyle.HasFlag(x));
            }

            var result = this.Set(ref this._editingStyle, newEditingStyle, nameof(this.EditingStyle));
            if (result) {
                this._editorGame.EditingStyle = this.EditingStyle;
            }

            var positionOfCurrent = -1;
            for (var i = 0; i < this._recentlyUsedEditingStyles.Length; i++) {
                if (this._recentlyUsedEditingStyles[i] == this.EditingStyle) {
                    positionOfCurrent = i;
                    break;
                }
            }

            if (positionOfCurrent > 0) {
                var recentlyUsedEditingStyles = this._recentlyUsedEditingStyles.ToList();
                recentlyUsedEditingStyles.Remove(this.EditingStyle);
                recentlyUsedEditingStyles.Insert(0, this.EditingStyle);

                if (recentlyUsedEditingStyles.Count == this._recentlyUsedEditingStyles.Length) {
                    for (var i = 0; i < this._recentlyUsedEditingStyles.Length; i++) {
                        this._recentlyUsedEditingStyles[i] = recentlyUsedEditingStyles.ElementAt(i);
                    }
                }
            }

            return result;
        }
    }
}
namespace Macabre2D.UI.GameEditorLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.GameEditorLibrary.Controls.SceneEditing;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Microsoft.Xna.Framework;
    using System;
    using System.ComponentModel;
    using System.Linq;

    public interface IMonoGameService {
        ComponentEditingStyle EditingStyle { get; set; }

        SceneEditor SceneEditor { get; }

        bool ShowGrid { get; set; }

        bool ShowSelection { get; set; }

        void CenterCamera();

        void ResetCamera();
    }

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly IComponentService _componentService;
        private readonly IFileService _fileService;
        private readonly IProjectService _projectService;

        private readonly ComponentEditingStyle[] _recentlyUsedEditingStyles = new[] {
            ComponentEditingStyle.Translation,
            ComponentEditingStyle.Scale,
            ComponentEditingStyle.Rotation,
            ComponentEditingStyle.Tile
        };

        private readonly SceneEditor _sceneEditor;
        private readonly ISceneService _sceneService;
        private ComponentEditingStyle _editingStyle = ComponentEditingStyle.Translation;
        private bool _hasSelectedGizmoBeenManuallyChanged = false;
        private bool _showGrid = true;
        private bool _showSelection = true;

        public MonoGameService(
            SceneEditor sceneEditor,
            IComponentService componentService,
            IFileService fileService,
            IProjectService projectService,
            ISceneService sceneService) {
            this._sceneEditor = sceneEditor;
            this._componentService = componentService;
            this._componentService.PropertyChanged += this.ComponentService_PropertyChanged;
            this._fileService = fileService;
            this._sceneService = sceneService;
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
            this._sceneEditor.CurrentScene = this._sceneService.CurrentScene?.SavableValue;
            this._projectService = projectService;
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._sceneEditor.AssetManager = this._projectService.CurrentProject?.AssetManager;
            this._sceneEditor.Settings = this._projectService.CurrentProject?.GameSettings;
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

        public SceneEditor SceneEditor {
            get {
                return this._sceneEditor;
            }
        }

        public bool ShowGrid {
            get {
                return this._showGrid;
            }

            set {
                this.Set(ref this._showGrid, value);
                this._sceneEditor.ShowGrid = this._showGrid;
            }
        }

        public bool ShowSelection {
            get {
                return this._showSelection;
            }

            set {
                this.Set(ref this._showSelection, value);
                this._sceneEditor.ShowSelection = this._showSelection;
            }
        }

        public void CenterCamera() {
            if (this._sceneEditor.CurrentCamera != null) {
                this._sceneEditor.CurrentCamera.LocalPosition = Vector2.Zero;
            }
        }

        public void ResetCamera() {
            this._sceneEditor.ResetCamera();
        }

        private void ComponentService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._componentService.SelectedItem)) {
                var componentEditingStyle = this._componentService.SelectedItem.GetEditingStyle();
                var editingStyleOverride = this.GetEditingStyleOverride(componentEditingStyle);

                if (this._componentService.SelectedItem != null && editingStyleOverride == ComponentEditingStyle.None && !this._hasSelectedGizmoBeenManuallyChanged && componentEditingStyle != ComponentEditingStyle.None) {
                    foreach (var editingStyle in this._recentlyUsedEditingStyles) {
                        if (componentEditingStyle.HasFlag(editingStyle)) {
                            editingStyleOverride = editingStyle;
                            break;
                        }
                    }
                }

                this.SetEditingStyle(editingStyleOverride);
            }
        }

        private ComponentEditingStyle GetEditingStyleOverride(ComponentEditingStyle componentEditingStyle) {
            var result = ComponentEditingStyle.None;

            if (componentEditingStyle.HasFlag(ComponentEditingStyle.Tile)) {
                result = ComponentEditingStyle.Tile;
            }
            else if (componentEditingStyle.HasFlag(this.EditingStyle)) {
                result = this.EditingStyle;
            }

            return result;
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IProjectService.CurrentProject)) {
                this._sceneEditor.AssetManager = this._projectService.CurrentProject?.AssetManager;
                this._sceneEditor.Settings = this._projectService.CurrentProject?.GameSettings;
                this.SetContentPath();
            }
        }

        private void SceneService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._sceneService.CurrentScene)) {
                this._sceneEditor.CurrentScene = this._sceneService.CurrentScene?.SavableValue;
            }
        }

        private void SetContentPath() {
            if (this._projectService.CurrentProject != null) {
                var desktopBuildConfiguration = this._projectService.CurrentProject.BuildConfigurations.FirstOrDefault(x => x.Platform == BuildPlatform.DesktopGL);
                this._sceneEditor.SetContentPath(desktopBuildConfiguration.GetCompiledContentPath(this._fileService.ProjectDirectoryPath, BuildMode.Debug));
            }
        }

        private bool SetEditingStyle(ComponentEditingStyle newEditingStyle) {
            this._hasSelectedGizmoBeenManuallyChanged = false;

            if (!((int)newEditingStyle).IsPowerOfTwo()) {
                newEditingStyle = Enum.GetValues(typeof(ComponentEditingStyle)).Cast<ComponentEditingStyle>().FirstOrDefault(x => x != ComponentEditingStyle.None && newEditingStyle.HasFlag(x));
            }

            var result = this.Set(ref this._editingStyle, newEditingStyle, nameof(this.EditingStyle));
            if (result) {
                this._sceneEditor.EditingStyle = this.EditingStyle;
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
namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : NotifyPropertyChanged, IMonoGameService {
        private readonly Dictionary<Guid, GizmoType> _componentIdToGizmoType = new Dictionary<Guid, GizmoType>();
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
        private bool _hasSelectedGizmoBeenManuallyChanged = false;
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
                if (this.SetSelectedGizmo(value)) {
                    this._hasSelectedGizmoBeenManuallyChanged = true;
                    this.CacheGizmoType(this._componentService.SelectedItem);
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

        private void CacheGizmoType(ComponentWrapper component) {
            // TODO: when we remove GizmoType, make sure the component is able
            if (component != null && this._gizmoTypeToMatchingEditingStyles.TryGetValue(this.SelectedGizmo, out var editingStyle) && (component.EditingStyle & editingStyle) != ComponentEditingStyle.None) {
                this._componentIdToGizmoType[component.Id] = this.SelectedGizmo;
            }
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            this.CacheGizmoType(e.OldValue);
            var cachedGizmoType = GizmoType.None;
            var hasCachedValue = e.NewValue != null && this._componentIdToGizmoType.TryGetValue(e.NewValue.Id, out cachedGizmoType);

            if (!this._hasSelectedGizmoBeenManuallyChanged && hasCachedValue && cachedGizmoType != GizmoType.None) {
                this.SetSelectedGizmo(cachedGizmoType);
            }
            else if (this.SelectedGizmo != GizmoType.None) {
                if (e.NewValue != null && this._gizmoTypeToMatchingEditingStyles.TryGetValue(this.SelectedGizmo, out var validEditingStyle)) {
                    if ((validEditingStyle & e.NewValue.EditingStyle) == ComponentEditingStyle.None) {
                        if (hasCachedValue) {
                            this.SetSelectedGizmo(cachedGizmoType);
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Translation) == ComponentEditingStyle.Translation) {
                            this.SetSelectedGizmo(GizmoType.Translation);
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Scale) == ComponentEditingStyle.Scale) {
                            this.SetSelectedGizmo(GizmoType.Scale);
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Rotation) == ComponentEditingStyle.Rotation) {
                            this.SetSelectedGizmo(GizmoType.Rotation);
                        }
                        else if ((e.NewValue.EditingStyle & ComponentEditingStyle.Tile) == ComponentEditingStyle.Tile) {
                            this.SetSelectedGizmo(GizmoType.Tile);
                        }
                        else {
                            this.SetSelectedGizmo(GizmoType.None);
                        }
                    }
                }
                else {
                    this.SetSelectedGizmo(GizmoType.None);
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

        private bool SetSelectedGizmo(GizmoType newGizmoType) {
            this._hasSelectedGizmoBeenManuallyChanged = false;
            var result = this.Set(ref this._selectedGizmo, newGizmoType, nameof(this.SelectedGizmo));
            if (result) {
                this._editorGame.SelectedGizmo = this.SelectedGizmo;
            }

            return result;
        }
    }
}
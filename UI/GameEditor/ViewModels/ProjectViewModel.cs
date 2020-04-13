namespace Macabre2D.UI.GameEditor.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Services;
    using Microsoft.Xna.Framework;

    public sealed class ProjectViewModel : NotifyPropertyChanged {
        private readonly IComponentService _componentService;
        private readonly IMonoGameService _monoGameService;
        private readonly IUndoService _undoService;

        public ProjectViewModel(
            IBusyService busyService,
            IComponentService componentService,
            IMonoGameService monoGameService,
            IProjectService projectService,
            IUndoService undoService) {
            this.BusyService = busyService;
            this._componentService = componentService;
            this._monoGameService = monoGameService;
            this.ProjectService = projectService;
            this._undoService = undoService;
            this.ProjectService.PropertyChanged += this.ProjectService_PropertyChanged;

            if (this.ProjectService.CurrentProject != null) {
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        public IBusyService BusyService { get; }

        public DisplayModes DefaultDisplayMode {
            get {
                return this.ProjectService.CurrentProject?.GameSettings.DefaultGraphicsSettings.DisplayMode ?? DisplayModes.Windowed;
            }

            set {
                var originalValue = this.DefaultDisplayMode;

                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.DefaultGraphicsSettings.DisplayMode = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.DefaultGraphicsSettings.DisplayMode = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.ErrorSpritesColor)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public Point DefaultResolution {
            get {
                return this.ProjectService.CurrentProject?.GameSettings.DefaultGraphicsSettings.Resolution ?? new Point(1920, 1080);
            }

            set {
                var originalValue = this.DefaultResolution;

                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.DefaultGraphicsSettings.Resolution = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.DefaultGraphicsSettings.Resolution = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.ErrorSpritesColor)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public Color ErrorSpritesColor {
            get {
                return this.ProjectService.CurrentProject?.GameSettings.ErrorSpritesColor ?? Color.HotPink;
            }

            set {
                var originalValue = this.ErrorSpritesColor;
                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.ErrorSpritesColor = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.ErrorSpritesColor = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.ErrorSpritesColor)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public Color FallbackBackgroundColor {
            get {
                return this.ProjectService.CurrentProject?.GameSettings.FallbackBackgroundColor ?? Color.Black;
            }

            set {
                var originalValue = this.FallbackBackgroundColor;
                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.FallbackBackgroundColor = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.FallbackBackgroundColor = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.FallbackBackgroundColor)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public int PixelsPerUnit {
            get {
                if (this.ProjectService.CurrentProject?.GameSettings is GameSettings settings) {
                    return settings.PixelsPerUnit;
                }

                return 1;
            }

            set {
                var originalValue = this.PixelsPerUnit;
                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.PixelsPerUnit = value;
                            this.ProjectService.HasChanges = true;
                            this._componentService.ResetSelectedItemBoundingArea();
                            this._monoGameService.ResetCamera();
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.PixelsPerUnit = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                            this._componentService.ResetSelectedItemBoundingArea();
                            this._monoGameService.ResetCamera();
                        },
                        () => this.RaisePropertyChanged(nameof(this.PixelsPerUnit)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public string ProjectName {
            get {
                return this.ProjectService.CurrentProject?.Name;
            }

            set {
                var originalValue = this.ProjectName;
                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.Name = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.Name = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.ProjectName)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public IProjectService ProjectService { get; }

        public SceneAsset SelectedStartUpSceneAsset {
            get {
                return this.ProjectService.CurrentProject?.StartUpSceneAsset;
            }

            set {
                var originalValue = this.SelectedStartUpSceneAsset;
                if (value != originalValue) {
                    var originalHasChanges = this.ProjectService.HasChanges;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.ProjectService.CurrentProject.StartUpSceneAsset = value;
                            this.ProjectService.HasChanges = true;
                        },
                        () => {
                            this.ProjectService.CurrentProject.StartUpSceneAsset = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
                        },
                        () => this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset)));

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.ProjectService.CurrentProject.StartUpSceneAsset)) {
                this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
            }
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IProjectService.CurrentProject) && this.ProjectService.CurrentProject != null) {
                this.RaisePropertyChangedEvents();
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        private void RaisePropertyChangedEvents() {
            this.RaisePropertyChanged(nameof(this.DefaultDisplayMode));
            this.RaisePropertyChanged(nameof(this.DefaultResolution));
            this.RaisePropertyChanged(nameof(this.ErrorSpritesColor));
            this.RaisePropertyChanged(nameof(this.FallbackBackgroundColor));
            this.RaisePropertyChanged(nameof(this.PixelsPerUnit));
            this.RaisePropertyChanged(nameof(this.ProjectName));
            this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
        }
    }
}
namespace Macabre2D.UI.ViewModels {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;

    public sealed class ProjectViewModel : NotifyPropertyChanged {
        private readonly IMonoGameService _monoGameService;

        private readonly IUndoService _undoService;

        public ProjectViewModel(
            IMonoGameService monoGameService,
            IProjectService projectService,
            IUndoService undoService) {
            this._monoGameService = monoGameService;
            this.ProjectService = projectService;
            this._undoService = undoService;
            this.ProjectService.PropertyChanged += this.ProjectService_PropertyChanged;

            if (this.ProjectService.CurrentProject != null) {
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
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

        public string Layer01Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer01);
            }

            set {
                this.UpdateLayer(Layers.Layer01, value);
            }
        }

        public string Layer02Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer02);
            }

            set {
                this.UpdateLayer(Layers.Layer02, value);
            }
        }

        public string Layer03Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer03);
            }

            set {
                this.UpdateLayer(Layers.Layer03, value);
            }
        }

        public string Layer04Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer04);
            }

            set {
                this.UpdateLayer(Layers.Layer04, value);
            }
        }

        public string Layer05Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer05);
            }

            set {
                this.UpdateLayer(Layers.Layer05, value);
            }
        }

        public string Layer06Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer06);
            }

            set {
                this.UpdateLayer(Layers.Layer06, value);
            }
        }

        public string Layer07Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer07);
            }

            set {
                this.UpdateLayer(Layers.Layer07, value);
            }
        }

        public string Layer08Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer08);
            }

            set {
                this.UpdateLayer(Layers.Layer08, value);
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
                            this._monoGameService.ResetCamera();
                        },
                        () => {
                            this.ProjectService.CurrentProject.GameSettings.PixelsPerUnit = originalValue;
                            this.ProjectService.HasChanges = originalHasChanges;
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
                    var undoCommand = new UndoCommand(
                        () => this.ProjectService.CurrentProject.Name = value,
                        () => this.ProjectService.CurrentProject.Name = originalValue,
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
                    var undoCommand = new UndoCommand(
                        () => this.ProjectService.CurrentProject.StartUpSceneAsset = value,
                        () => this.ProjectService.CurrentProject.StartUpSceneAsset = originalValue,
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
            if (e.PropertyName == nameof(this.ProjectService.CurrentProject)) {
                this.RaisePropertyChangedEvents();
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        private void RaisePropertyChangedEvents() {
            this.RaisePropertyChanged(nameof(this.ErrorSpritesColor));
            this.RaisePropertyChanged(nameof(this.FallbackBackgroundColor));
            this.RaisePropertyChanged(nameof(this.PixelsPerUnit));
            this.RaisePropertyChanged(nameof(this.ProjectName));
            this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
            this.RaisePropertyChanged(nameof(this.Layer01Name));
            this.RaisePropertyChanged(nameof(this.Layer02Name));
            this.RaisePropertyChanged(nameof(this.Layer03Name));
            this.RaisePropertyChanged(nameof(this.Layer04Name));
            this.RaisePropertyChanged(nameof(this.Layer05Name));
            this.RaisePropertyChanged(nameof(this.Layer06Name));
            this.RaisePropertyChanged(nameof(this.Layer07Name));
            this.RaisePropertyChanged(nameof(this.Layer08Name));
        }

        private void UpdateLayer(Layers layer, string name) {
            var originalName = GameSettings.Instance.Layers.GetLayerName(layer);
            var originalHasChanges = this.ProjectService.HasChanges;

            var undoCommand = new UndoCommand(() => {
                GameSettings.Instance.Layers.SetLayerName(layer, name);
                this.ProjectService.HasChanges = true;
            }, () => {
                GameSettings.Instance.Layers.SetLayerName(layer, originalName);
                this.ProjectService.HasChanges = originalHasChanges;
            });

            this._undoService.Do(undoCommand);
        }
    }
}
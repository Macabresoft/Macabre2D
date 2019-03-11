namespace Macabre2D.UI.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class ProjectViewModel : NotifyPropertyChanged {
        private readonly RelayCommand _deleteAssetCommand;
        private readonly IDialogService _dialogService;
        private readonly IMonoGameService _monoGameService;
        private readonly ISceneService _sceneService;
        private readonly Serializer _serializer;
        private readonly IUndoService _undoService;
        private Asset _selectedAsset;

        public ProjectViewModel(
            IDialogService dialogService,
            IMonoGameService monoGameService,
            IProjectService projectService,
            ISceneService sceneService,
            Serializer serializer,
            IUndoService undoService) {
            this._dialogService = dialogService;
            this._monoGameService = monoGameService;
            this.ProjectService = projectService;
            this._sceneService = sceneService;
            this._serializer = serializer;
            this._undoService = undoService;

            this.AddAssetCommand = new RelayCommand(this.AddAsset);
            this._deleteAssetCommand = new RelayCommand(async () => await this.DeleteAsset(), () => this.SelectedAsset?.Parent != null);
            this.OpenSceneCommand = new RelayCommand<Asset>(this.OpenScene, asset => asset.Type == AssetType.Scene);

            this.ProjectService.PropertyChanged += this.ProjectService_PropertyChanged;

            if (this.ProjectService.CurrentProject != null) {
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        public ICommand AddAssetCommand { get; }

        public ICommand DeleteAssetCommand {
            get {
                return this._deleteAssetCommand;
            }
        }

        public Color FallbackBackgroundColor {
            get {
                return this.ProjectService.CurrentProject?.GameSettings.FallbackBackgroundColor ?? Color.HotPink;
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

        public ICommand OpenSceneCommand { get; }

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

        public Asset SelectedAsset {
            get {
                return this._selectedAsset;
            }

            set {
                if (this.Set(ref this._selectedAsset, value)) {
                    this.RaisePropertyChanged(nameof(this.SelectedFontAsset));
                    this.RaisePropertyChanged(nameof(this.SelectedImageAsset));
                    this.RaisePropertyChanged(nameof(this.SelectedSpriteAnimationAsset));
                    this.RaisePropertyChanged(nameof(this.SelectedSpriteWrapper));
                    this._deleteAssetCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public FontAsset SelectedFontAsset {
            get {
                return this.SelectedAsset as FontAsset;
            }
        }

        public ImageAsset SelectedImageAsset {
            get {
                return this.SelectedAsset as ImageAsset;
            }
        }

        public SpriteAnimationAsset SelectedSpriteAnimationAsset {
            get {
                return this.SelectedAsset as SpriteAnimationAsset;
            }
        }

        public SpriteWrapper SelectedSpriteWrapper {
            get {
                return this.SelectedAsset as SpriteWrapper;
            }
        }

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

        private void AddAsset() {
            var result = this._dialogService.ShowSelectTypeAndNameDialog(typeof(AddableAsset<>));
            if (result.Type != null && !string.IsNullOrEmpty(result.Name)) {
                var asset = Activator.CreateInstance(result.Type) as AddableAsset;

                if (!result.Name.ToUpper().EndsWith(asset.FileExtension.ToUpper())) {
                    asset.Name = $"{result.Name}{asset.FileExtension}";
                }
                else {
                    asset.Name = result.Name;
                }

                var selectedAsset = this.SelectedAsset;
                if (selectedAsset != null) {
                    var undoCommand = new UndoCommand(
                        () => {
                            if (selectedAsset is FolderAsset folderAsset) {
                                folderAsset.AddChild(asset);
                            }
                            else {
                                selectedAsset.Parent.AddChild(asset);
                            }

                            asset.Save(this._serializer);
                        }, () => {
                            asset.Parent.RemoveChild(asset);
                            asset.Delete();
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.ProjectService.CurrentProject.StartUpSceneAsset)) {
                this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
            }
        }

        private async Task DeleteAsset() {
            if (this.SelectedAsset != null) {
                if (this._dialogService.ShowYesNoMessageBox("Delete Asset", $"Delete {this.SelectedAsset.Name}? This action cannot be undone.")) {
                    this.SelectedAsset.Delete();
                    this.SelectedAsset = null;
                    await this.ProjectService.SaveProject();
                }
            }
        }

        private void OpenScene(Asset asset) {
            if (asset is SceneAsset scene) {
                this._sceneService.LoadScene(this.ProjectService.CurrentProject, scene);
            }
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.ProjectService.CurrentProject)) {
                this.RaisePropertyChangedEvents();
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        private void RaisePropertyChangedEvents() {
            this.RaisePropertyChanged(nameof(this.FallbackBackgroundColor));
            this.RaisePropertyChanged(nameof(this.PixelsPerUnit));
            this.RaisePropertyChanged(nameof(this.ProjectName));
            this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
        }
    }
}
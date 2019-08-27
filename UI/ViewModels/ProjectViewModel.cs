namespace Macabre2D.UI.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class ProjectViewModel : NotifyPropertyChanged {
        private readonly RelayCommand _addAssetCommand;
        private readonly RelayCommand _deleteAssetCommand;
        private readonly IDialogService _dialogService;
        private readonly IMonoGameService _monoGameService;
        private readonly RelayCommand _newFolderCommand;
        private readonly ISceneService _sceneService;
        private readonly Serializer _serializer;
        private readonly IUndoService _undoService;

        public ProjectViewModel(
            IAssetService assetService,
            IDialogService dialogService,
            IMonoGameService monoGameService,
            IProjectService projectService,
            ISceneService sceneService,
            Serializer serializer,
            IUndoService undoService) {
            this.AssetService = assetService;
            this._dialogService = dialogService;
            this._monoGameService = monoGameService;
            this.ProjectService = projectService;
            this._sceneService = sceneService;
            this._serializer = serializer;
            this._undoService = undoService;

            this._addAssetCommand = new RelayCommand(this.AddAsset, () => this.AssetService.SelectedAsset is FolderAsset);
            this._deleteAssetCommand = new RelayCommand(async () => await this.DeleteAsset(), () => this.AssetService.SelectedAsset?.Parent != null);
            this._newFolderCommand = new RelayCommand(this.CreateNewFolder, () => this.AssetService.SelectedAsset is FolderAsset);
            this.OpenSceneCommand = new RelayCommand<Asset>(this.OpenScene, asset => asset.Type == AssetType.Scene);
            this.AssetService.PropertyChanged += this.AssetService_PropertyChanged;
            this.ProjectService.PropertyChanged += this.ProjectService_PropertyChanged;

            if (this.ProjectService.CurrentProject != null) {
                this.ProjectService.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
            }
        }

        public ICommand AddAssetCommand {
            get {
                return this._addAssetCommand;
            }
        }

        public IAssetService AssetService { get; }

        public ICommand DeleteAssetCommand {
            get {
                return this._deleteAssetCommand;
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

        public ICommand NewFolderCommand {
            get {
                return this._newFolderCommand;
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
            var result = this._dialogService.ShowSelectTypeAndNameDialog(typeof(AddableAsset), "Select an Asset");
            if (result.Type != null && !string.IsNullOrEmpty(result.Name)) {
                var asset = Activator.CreateInstance(result.Type) as AddableAsset;
                asset.RequiresCreation = true;

                if (!result.Name.ToUpper().EndsWith(asset.FileExtension.ToUpper())) {
                    asset.Name = $"{result.Name}{asset.FileExtension}";
                }
                else {
                    asset.Name = result.Name;
                }

                var selectedAsset = this.AssetService.SelectedAsset ?? this.ProjectService.CurrentProject.AssetFolder;
                if (selectedAsset != null) {
                    var undoCommand = new UndoCommand(
                        () => {
                            if (selectedAsset is FolderAsset folderAsset) {
                                folderAsset.AddChild(asset);
                            }
                            else {
                                selectedAsset.Parent.AddChild(asset);
                            }

                            asset.Save(this._serializer, this.ProjectService.CurrentProject.AssetManager);
                            asset.Refresh(this.ProjectService.CurrentProject.AssetManager);
                        }, () => {
                            asset.Delete();
                            asset.Parent.RemoveChild(asset);
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private void AssetService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IAssetService.SelectedAsset)) {
                this._addAssetCommand.RaiseCanExecuteChanged();
                this._deleteAssetCommand.RaiseCanExecuteChanged();
                this._newFolderCommand.RaiseCanExecuteChanged();
            }
        }

        private void CreateNewFolder() {
            if (this.AssetService.SelectedAsset is FolderAsset parent) {
                var counter = 0;
                var name = FileHelper.NewFolderDefaultName;
                while (parent.Children.Any(x => string.Equals(x.NameWithoutExtension, name, StringComparison.OrdinalIgnoreCase))) {
                    counter++;
                    name = $"{FileHelper.NewFolderDefaultName} ({counter})";
                }

                var asset = new FolderAsset(name);

                var undoCommand = new UndoCommand(
                    () => {
                        parent.AddChild(asset);
                        Directory.CreateDirectory(asset.GetPath());
                    }, () => {
                        asset.Delete();
                        parent.RemoveChild(asset);
                    });

                this._undoService.Do(undoCommand);
            }
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.ProjectService.CurrentProject.StartUpSceneAsset)) {
                this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
            }
        }

        private async Task DeleteAsset() {
            if (this.AssetService.SelectedAsset != null && this._dialogService.ShowYesNoMessageBox("Delete Asset", $"Delete {this.AssetService.SelectedAsset.Name}? This action cannot be undone.")) {
                this.AssetService.SelectedAsset.Delete();
                this.AssetService.SelectedAsset = this.ProjectService.CurrentProject.AssetFolder;
                await this.ProjectService.SaveProject();
                this._undoService.Clear();
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
            this.RaisePropertyChanged(nameof(this.ErrorSpritesColor));
            this.RaisePropertyChanged(nameof(this.FallbackBackgroundColor));
            this.RaisePropertyChanged(nameof(this.PixelsPerUnit));
            this.RaisePropertyChanged(nameof(this.ProjectName));
            this.RaisePropertyChanged(nameof(this.SelectedStartUpSceneAsset));
        }
    }
}
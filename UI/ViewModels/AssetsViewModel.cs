namespace Macabre2D.UI.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class AssetsViewModel : NotifyPropertyChanged {
        private readonly RelayCommand _addAssetCommand;
        private readonly IBusyService _busyService;
        private readonly RelayCommand _deleteAssetCommand;
        private readonly IDialogService _dialogService;
        private readonly RelayCommand _newFolderCommand;
        private readonly RelayCommand _reloadAssetCommand;
        private readonly ISceneService _sceneService;
        private readonly Serializer _serializer;
        private readonly IUndoService _undoService;

        public AssetsViewModel(
            IAssetService assetService,
            IBusyService busyService,
            IDialogService dialogService,
            IProjectService projectService,
            ISceneService sceneService,
            Serializer serializer,
            IUndoService undoService) {
            this.AssetService = assetService;
            this._busyService = busyService;
            this._dialogService = dialogService;
            this.ProjectService = projectService;
            this._sceneService = sceneService;
            this._serializer = serializer;
            this._undoService = undoService;

            this._addAssetCommand = new RelayCommand(this.AddAsset, () => this.AssetService.SelectedAsset is FolderAsset);
            this._deleteAssetCommand = new RelayCommand(async () => await this.DeleteAsset(), () => this.AssetService.SelectedAsset?.Parent != null);
            this._newFolderCommand = new RelayCommand(this.CreateNewFolder, () => this.AssetService.SelectedAsset is FolderAsset);
            this.OpenSceneCommand = new RelayCommand<Asset>(this.OpenScene, asset => typeof(SceneAsset).IsAssignableFrom(asset.GetType()));
            this._reloadAssetCommand = new RelayCommand(async () => await this.ReloadAsset(), () => this.AssetService.SelectedAsset is IReloadableAsset);
            this.AssetService.PropertyChanged += this.AssetService_PropertyChanged;
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

        public ICommand NewFolderCommand {
            get {
                return this._newFolderCommand;
            }
        }

        public ICommand OpenSceneCommand { get; }

        public IProjectService ProjectService { get; }

        public ICommand ReloadAssetCommand {
            get {
                return this._reloadAssetCommand;
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
                            this.AssetService.BuildAssets(this.ProjectService.CurrentProject.EditorConfiguration, BuildMode.Debug, asset);
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
                this._reloadAssetCommand.RaiseCanExecuteChanged();
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
                var hasChanges = this.ProjectService.HasChanges;
                var undoCommand = new UndoCommand(
                    () => {
                        this.ProjectService.HasChanges = true;
                        parent.AddChild(asset);
                        Directory.CreateDirectory(asset.GetPath());
                    }, () => {
                        asset.Delete();
                        parent.RemoveChild(asset);
                        this.ProjectService.HasChanges = hasChanges;
                    });

                this._undoService.Do(undoCommand);
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

        private async Task ReloadAsset() {
            if (this.AssetService.SelectedAsset is IReloadableAsset reloadable) {
                var task = Task.Run(async () => {
                    var assetIds = reloadable.GetOwnedAssetIds();
                    foreach (var assetId in assetIds) {
                        this.ProjectService.CurrentProject.AssetManager.Unload(assetId);
                    }

                    if (this.AssetService.SelectedAsset is MetadataAsset metadataAsset) {
                        metadataAsset.Save(this._serializer, this.ProjectService.CurrentProject.AssetManager);
                    }

                    await this.AssetService.BuildAssets(this.ProjectService.CurrentProject.EditorConfiguration, BuildMode.Debug, this.AssetService.SelectedAsset);
                    reloadable.Reload();
                });

                await this._busyService.PerformTask(task, true);
            }
        }
    }
}
namespace Macabre2D.UI.Library.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using Macabre2D.Framework;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class AssetsViewModel : NotifyPropertyChanged {
        private readonly IDialogService _dialogService;
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;

        public AssetsViewModel(
            IAssetService assetService,
            IBusyService busyService,
            IDialogService dialogService,
            IProjectService projectService,
            ISceneService sceneService,
            IUndoService undoService) {
            this.AssetService = assetService;
            this.BusyService = busyService;
            this._dialogService = dialogService;
            this.ProjectService = projectService;
            this._sceneService = sceneService;
            this._undoService = undoService;

            this.AddAssetCommand = new RelayCommand<Asset>(this.AddAsset, x => x is FolderAsset);
            this.DeleteAssetCommand = new RelayCommand<Asset>(async x => await this.DeleteAsset(x), this.CanDeleteAsset);
            this.ImportAssetCommand = new RelayCommand<Asset>(this.ImportAsset, x => x is FolderAsset);
            this.NewFolderCommand = new RelayCommand<Asset>(this.CreateNewFolder, x => x is FolderAsset);
            this.OpenSceneCommand = new RelayCommand<Asset>(this.OpenScene, asset => typeof(SceneAsset).IsAssignableFrom(asset.GetType()));
        }

        public ICommand AddAssetCommand { get; }

        public IAssetService AssetService { get; }

        public IBusyService BusyService { get; }

        public ICommand DeleteAssetCommand { get; }

        public ICommand ImportAssetCommand { get; }

        public ICommand NewFolderCommand { get; }

        public ICommand OpenSceneCommand { get; }

        public IProjectService ProjectService { get; }

        private void AddAsset(Asset selectedAsset) {
            var result = this._dialogService.ShowSelectTypeAndNameDialog(typeof(AddableAsset), "Select an Asset");
            if (result.Type != null && !string.IsNullOrEmpty(result.Name)) {
                var asset = Activator.CreateInstance(result.Type) as AddableAsset;

                if (!result.Name.ToUpper().EndsWith(asset.FileExtension.ToUpper())) {
                    asset.Name = $"{result.Name}{asset.FileExtension}";
                }
                else {
                    asset.Name = result.Name;
                }

                if (selectedAsset != null) {
                    var undoCommand = new UndoCommand(
                        () => {
                            if (selectedAsset is FolderAsset folderAsset) {
                                folderAsset.AddChild(asset);
                            }
                            else {
                                selectedAsset.Parent.AddChild(asset);
                            }

                            asset.Save();
                            this.AssetService.BuildAssets(this.ProjectService.CurrentProject.EditorConfiguration, BuildMode.Debug, asset);
                            asset.Refresh(this.ProjectService.CurrentProject.AssetManager);
                            this.AssetService.SelectedAsset = asset;
                        }, () => {
                            asset.Delete();
                            asset.Parent.RemoveChild(asset);
                            this.AssetService.SelectedAsset = selectedAsset;
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private bool CanDeleteAsset(Asset asset) {
            return asset?.Parent != null && asset.Id != this._sceneService.CurrentScene?.Id;
        }

        private void CreateNewFolder(Asset selectedAsset) {
            if (selectedAsset is FolderAsset parent) {
                var counter = 0;
                var name = FileHelper.NewFolderDefaultName;
                while (parent.Children.Any(x => string.Equals(x.NameWithoutExtension, name, StringComparison.OrdinalIgnoreCase))) {
                    counter++;
                    name = $"{FileHelper.NewFolderDefaultName} ({counter})";
                }

                var newAsset = new FolderAsset(name);
                var hasChanges = this.ProjectService.HasChanges;
                var undoCommand = new UndoCommand(
                    () => {
                        this.ProjectService.HasChanges = true;
                        parent.AddChild(newAsset);
                        Directory.CreateDirectory(newAsset.GetPath());
                        this.AssetService.SelectedAsset = newAsset;
                    }, () => {
                        newAsset.Delete();
                        parent.RemoveChild(newAsset);
                        this.ProjectService.HasChanges = hasChanges;
                        this.AssetService.SelectedAsset = parent;
                    });

                this._undoService.Do(undoCommand);
            }
        }

        private async Task DeleteAsset(Asset asset) {
            if (asset != null && this._dialogService.ShowYesNoMessageBox("Delete Asset", $"Delete {asset.Name}? This action cannot be undone.")) {
                var parent = asset.Parent ?? this.ProjectService.CurrentProject.AssetFolder;
                asset.Delete();
                this.AssetService.SelectedAsset = parent;
                await this.ProjectService.SaveProject();
                this._undoService.Clear();
            }
        }

        private void ImportAsset(Asset selectedAsset) {
            if (selectedAsset is FolderAsset folder) {
                var filter = "All files (*.*)|*.*|";

                foreach (var extension in FileHelper.ImageExtensions) {
                    filter += $"(*{extension})|*{extension}|";
                }

                foreach (var extension in FileHelper.AudioExtensions) {
                    filter += $"(*{extension})|*{extension}|";
                }

                filter = filter.TrimEnd('|');

                if (this._dialogService.ShowFileBrowser(filter, out var path) && File.Exists(path)) {
                    var fileName = Path.GetFileName(path);
                    var newPath = Path.Combine(folder.GetPath(), fileName);

                    // TODO: validate file name.
                    if (!File.Exists(newPath)) {
                        File.Copy(path, newPath);
                        var asset = FolderAsset.GetAssetFromFilePath(newPath);
                        folder.AddChild(asset);
                    }
                }
            }
        }

        private void OpenScene(Asset asset) {
            if (asset is SceneAsset scene) {
                this._sceneService.LoadScene(this.ProjectService.CurrentProject, scene);
            }
        }
    }
}
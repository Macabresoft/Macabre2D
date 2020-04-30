namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.AssetEditors;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services.Content;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public interface IAssetService : INotifyPropertyChanged {
        Asset SelectedAsset { get; set; }

        Task<bool> BuildAssets(BuildConfiguration configuration, BuildMode mode, params Asset[] assets);

        void ChangeAssetParent(Asset assetToMove, FolderAsset newParent);

        DependencyObject GetEditor(Asset asset);

        void RenameAsset(Asset asset, string newName);
    }

    public sealed class AssetService : NotifyPropertyChanged, IAssetService {
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;
        private readonly ILoggingService _loggingService;
        private readonly IUndoService _undoService;
        private Asset _selectedAsset;

        public AssetService(
            IDialogService dialogService,
            IFileService fileService,
            ILoggingService loggingService,
            IUndoService undoService) {
            this._dialogService = dialogService;
            this._fileService = fileService;
            this._loggingService = loggingService;
            this._undoService = undoService;
        }

        public Asset SelectedAsset {
            get {
                return this._selectedAsset;
            }

            set {
                this.Set(ref this._selectedAsset, value);
            }
        }

        public async Task<bool> BuildAssets(BuildConfiguration configuration, BuildMode mode, params Asset[] assets) {
            var result = true;

            // TODO: this is probably broken because of output directory.
            await Task.Run(() => {
                var tempPath = Path.Combine(this._fileService.ProjectDirectoryPath, "temp");
                var dllPaths = new[] {
                    $@"..\..\Intermediary\{configuration.Platform}\bin\{mode}\Newtonsoft.Json.dll",
                    $@"..\..\Intermediary\{configuration.Platform}\bin\{mode}\Macabre2D.Framework.dll",
                    $@"..\..\Intermediary\{configuration.Platform}\bin\{mode}\Macabre2D.Project.Gameplay.dll"
                };

                var outputDirectory = configuration.GetCompiledContentPath(this._fileService.ProjectDirectoryPath, mode);
                configuration.CreateContentFile(this._fileService.ProjectDirectoryPath, assets, true, dllPaths);
                var contentFilePath = Path.Combine(this._fileService.ProjectDirectoryPath, $"{FileHelper.TempName}{FileHelper.ContentExtension}");
                Directory.CreateDirectory(outputDirectory);

                var exitCode = ContentBuilder.BuildContent(
                    out var exception,
                    $"/platform:{configuration.Platform.ToString()}",
                    $@"/outputDir:{outputDirectory}",
                    $"/workingDir:{this._fileService.ProjectDirectoryPath}",
                    $"/incremental:{true}",
                    $"/@:{contentFilePath}");

                if (exitCode != 0) {
                    result = false;
                    this._loggingService.LogError($"Content could not be built in '{mode.ToString()}' mode: {exception?.Message}");
                }
            });

            return result;
        }

        public void ChangeAssetParent(Asset assetToMove, FolderAsset newParent) {
            if (newParent == null) {
                throw new ArgumentNullException(nameof(newParent));
            }

            if (this.ValidateNewParent(assetToMove, newParent, out var newName)) {
                var originalName = assetToMove.Name;
                var originalPath = assetToMove.GetPath();
                var originalParent = assetToMove.Parent;
                var newPath = originalPath;

                var undoCommand = new UndoCommand(() => {
                    assetToMove.Name = newName;
                    assetToMove.Parent = newParent;
                    newPath = assetToMove.GetPath();
                    this.MoveAsset(assetToMove, originalPath);
                }, () => {
                    assetToMove.Name = originalName;
                    assetToMove.Parent = originalParent;
                    this.MoveAsset(assetToMove, newPath);
                });

                this._undoService.Do(undoCommand);
            }
        }

        public DependencyObject GetEditor(Asset asset) {
            DependencyObject editor = null;
            if (asset is ImageAsset imageAsset) {
                editor = new ImageAssetEditor {
                    Asset = imageAsset
                };
            }
            else if (asset is SpriteAnimationAsset spriteAnimation) {
                editor = new SpriteAnimationAssetEditor {
                    Asset = spriteAnimation
                };
            }
            else if (asset is FontAsset fontAsset) {
                editor = new FontAssetEditor {
                    Asset = fontAsset
                };
            }
            else if (asset is AutoTileSetAsset autoTileSetAsset) {
                editor = new AutoTileSetAssetEditor {
                    Asset = autoTileSetAsset
                };
            }

            return editor;
        }

        public void RenameAsset(Asset asset, string newName) {
            if (this.ValidateAssetName(newName, asset, asset.Parent, out var validName)) {
                var originalPath = asset.GetPath();
                var newPath = Path.Combine(Path.GetDirectoryName(originalPath), validName);
                var originalName = asset.Name;

                var undoCommand = new UndoCommand(() => {
                    asset.Name = validName;
                    this.MoveAsset(asset, originalPath);
                }, () => {
                    asset.Name = originalName;
                    this.MoveAsset(asset, newPath);
                });

                this._undoService.Do(undoCommand);
            }
        }

        private void MoveAsset(Asset asset, string originalPath) {
            if (asset is FolderAsset folder) {
                this.MoveFolder(folder, originalPath);
            }
            else {
                var newPath = asset.GetPath();
                if (!string.IsNullOrEmpty(originalPath) && !string.IsNullOrEmpty(newPath) && File.Exists(originalPath)) {
                    File.Move(originalPath, newPath);
                    asset.ResetContentPath();
                }
            }
        }

        private void MoveFolder(FolderAsset folder, string originalPath) {
            var newPath = folder.GetPath();
            if (!Directory.Exists(newPath)) {
                Directory.CreateDirectory(newPath);

                foreach (var child in folder.Children) {
                    this.MoveAsset(child, Path.Combine(originalPath, child.Name));
                }

                Directory.Delete(originalPath, true);
            }
        }

        private bool ValidateAssetName(string name, Asset asset, FolderAsset parentAsset, out string newName) {
            var result = !string.IsNullOrWhiteSpace(name);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            if (string.IsNullOrWhiteSpace(nameWithoutExtension)) {
                nameWithoutExtension = name;
            }

            newName = name;

            if (!result || parentAsset?.Children.Any(x => string.Equals(x.NameWithoutExtension, nameWithoutExtension, StringComparison.OrdinalIgnoreCase)) == true) {
                if (this._dialogService.ShowAssetNameChangeDialog(name, asset, parentAsset, out newName)) {
                    result = this.ValidateAssetName(newName, asset, parentAsset, out newName);
                }
                else {
                    result = false;
                }
            }

            return result;
        }

        private bool ValidateNewParent(Asset assetToMove, FolderAsset newParent, out string newName) {
            var result = false;
            newName = assetToMove.Name;

            if (assetToMove.Parent == null || newParent.Id != assetToMove.Parent.Id) {
                result = this.ValidateAssetName(assetToMove.Name, assetToMove, newParent, out newName);
            }

            return result;
        }
    }
}
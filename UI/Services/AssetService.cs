namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Controls.AssetEditors;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;

    public sealed class AssetService : NotifyPropertyChanged, IAssetService {
        private readonly IDialogService _dialogService;
        private readonly Serializer _serializer;
        private readonly IUndoService _undoService;
        private Asset _selectedAsset;

        public AssetService(
            IDialogService dialogService,
            Serializer serializer,
            IUndoService undoService) {
            this._dialogService = dialogService;
            this._serializer = serializer;
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

        public void ChangeAssetParent(Asset assetToMove, FolderAsset newParent) {
            if (newParent == null) {
                throw new ArgumentNullException(nameof(newParent));
            }

            if (this.ValidateNewParent(assetToMove, newParent)) {
                var originalPath = assetToMove.GetPath();
                var originalParent = assetToMove.Parent;
                var newPath = originalPath;

                var undoCommand = new UndoCommand(async () => {
                    assetToMove.Parent = newParent;
                    newPath = assetToMove.GetPath();
                    this.MoveAsset(assetToMove, originalPath);
                }, () => {
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
            else if (asset is SpriteWrapper spriteWrapper) {
                editor = new SpriteWrapperEditor {
                    ImageAsset = spriteWrapper.ImageAsset,
                    SpriteWrapper = spriteWrapper
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

            return editor;
        }

        public void RenameAsset(Asset asset, string newName) {
            if (this.ValidateAssetName(newName, asset.Parent, out var validName)) {
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
            else if (!(asset is SpriteWrapper)) {
                var newPath = asset.GetPath();
                if (!string.IsNullOrEmpty(originalPath) && !string.IsNullOrEmpty(newPath) && File.Exists(originalPath)) {
                    File.Move(originalPath, newPath);
                    asset.ResetContentPath(newPath);
                }

                if (asset is MetadataAsset metadataAsset) {
                    this.MoveMetadata(originalPath, newPath);
                    metadataAsset.Save(this._serializer);
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

        private void MoveMetadata(string originalPath, string newPath) {
            var originalMetadataPath = MetadataAsset.GetMetadataPath(originalPath);
            var newMetadataPath = MetadataAsset.GetMetadataPath(newPath);

            if (File.Exists(originalMetadataPath)) {
                File.Move(originalMetadataPath, newMetadataPath);
            }
        }

        private bool ValidateAssetName(string name, FolderAsset parentAsset, out string newName) {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            var result = !string.IsNullOrWhiteSpace(nameWithoutExtension);
            newName = name;

            if (!result || parentAsset.Children.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))) {
                var extension = Path.GetExtension(name);
                if (this._dialogService.ShowNameChangeDialog(nameWithoutExtension, extension, "Invalid Name, Choose a New One", out var chosenName)) {
                    result = this.ValidateAssetName($"{chosenName}{extension}", parentAsset, out newName);
                }
                else {
                    result = false;
                }
            }

            return result;
        }

        private bool ValidateNewParent(Asset assetToMove, FolderAsset newParent) {
            var result = false;
            if (assetToMove.Parent == null || newParent.Id != assetToMove.Parent.Id) {
                result = this.ValidateAssetName(assetToMove.Name, newParent, out var newName);
                if (!string.Equals(assetToMove.Name, newName, StringComparison.OrdinalIgnoreCase)) {
                    assetToMove.Name = newName;
                }
            }

            return result;
        }
    }
}
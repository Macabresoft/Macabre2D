using Macabre2D.UI.Models;
using Macabre2D.UI.Models.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Macabre2D.UI.ViewModels.Dialogs {

    public sealed class AssetNameChangeViewModel : OKCancelDialogViewModel {
        private readonly Asset _asset;
        private readonly FolderAsset _parent;
        private string _newName;

        public AssetNameChangeViewModel(string name, string extension, Asset asset, FolderAsset parent) {
            this._asset = asset ?? throw new ArgumentNullException(nameof(asset));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.Extension = string.IsNullOrWhiteSpace(extension) ? null : extension;
            this.NewName = name;
        }

        public string Extension { get; }

        [FileNameValidation]
        public string NewName {
            get {
                return this._newName;
            }

            set {
                this.Set(ref this._newName, value);
                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        protected override IEnumerable<string> RunCustomValidation() {
            var errors = base.RunCustomValidation().ToList();
            if (this._asset is FolderAsset folder) {
                var peerAssets = this._parent.Children.Where(x => x.Id != this._asset.Id);

                foreach (var peerAsset in peerAssets) {
                    if (string.Equals(this.NewName, peerAsset.Name, StringComparison.OrdinalIgnoreCase)) {
                        errors.Add($"'{this.NewName}' is already in use.");
                        break;
                    }
                }
            }
            else {
                var peerAssets = this._parent.Children.Where(x => x.Id != this._asset.Id);

                if (!string.IsNullOrWhiteSpace(this.Extension)) {
                    peerAssets = peerAssets.Where(x => !(x is FolderAsset));
                }

                foreach (var peerAsset in peerAssets) {
                    var compareToName = Path.GetFileNameWithoutExtension(peerAsset.Name);
                    if (string.IsNullOrEmpty(compareToName)) {
                        compareToName = peerAsset.Name;
                    }

                    if (string.Equals(this.NewName, compareToName, StringComparison.OrdinalIgnoreCase)) {
                        errors.Add($"'{this.NewName}' is already in use.");
                    }
                }
            }

            return errors;
        }
    }
}
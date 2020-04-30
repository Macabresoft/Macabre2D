namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Models.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class AssetNameChangeViewModel : OKCancelDialogViewModel {
        private readonly Asset _asset;
        private readonly FolderAsset _parent;
        private string _newName;

        public AssetNameChangeViewModel(string name, string extension, Asset asset, FolderAsset parent) {
            this._asset = asset ?? throw new ArgumentNullException(nameof(asset));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.Extension = string.IsNullOrWhiteSpace(extension) ? null : extension;
            this.NewName = name;

            var unavailableNames = new List<string>();
            if (this._asset is FolderAsset) {
                unavailableNames.AddRange(this._parent.Children.Where(x => x.Id != this._asset.Id).Select(x => x.Name));
            }
            else if (!string.IsNullOrWhiteSpace(this.Extension)) {
                unavailableNames.AddRange(this._parent.Children.Where(x => x.Id != this._asset.Id && !(x is FolderAsset)).Select(x => x.Name.WithoutExtension()));
            }
            else {
                unavailableNames.AddRange(this._parent.Children.Where(x => x.Id != this._asset.Id).Select(x => x.Name.WithoutExtension()));
            }

            this.UnavailableNames = unavailableNames;
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

        public ICollection<string> UnavailableNames { get; }

        protected override IEnumerable<string> RunCustomValidation() {
            var errors = base.RunCustomValidation().ToList();
            if (this._asset is FolderAsset folder) {
                var peerAssets = this._parent.Children.Where(x => x.Id != this._asset.Id);

                foreach (var unavailableName in this.UnavailableNames) {
                    if (string.Equals(this.NewName, unavailableName, StringComparison.OrdinalIgnoreCase)) {
                        errors.Add($"'{this.NewName}' is already in use.");
                        break;
                    }
                }
            }
            else {
                foreach (var unavailableName in this.UnavailableNames) {
                    if (string.Equals(this.NewName, unavailableName, StringComparison.OrdinalIgnoreCase)) {
                        errors.Add($"'{this.NewName}' is already in use.");
                        break;
                    }
                }
            }

            return errors;
        }
    }
}
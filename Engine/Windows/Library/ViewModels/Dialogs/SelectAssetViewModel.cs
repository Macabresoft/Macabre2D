namespace Macabre2D.Engine.Windows.ViewModels.Dialogs {

    using Macabre2D.Engine.Windows.Models;
    using System;

    public sealed class SelectAssetViewModel : OKCancelDialogViewModel {
        private Asset _selectedAsset;

        public SelectAssetViewModel(Project project, Type assetType, bool allowNull) {
            this.Project = project;
            this.AssetType = assetType;
            this.AllowNull = allowNull;
        }

        public bool AllowNull { get; }

        public Type AssetType { get; }

        public Project Project { get; }

        public Asset SelectedAsset {
            get {
                return this._selectedAsset;
            }

            set {
                if (this.Set(ref this._selectedAsset, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        protected override bool CanExecuteOKCommand() {
            return this.SelectedAsset is NullAsset || (this.SelectedAsset != null && this.AssetType.IsAssignableFrom(this.SelectedAsset.GetType()));
        }
    }
}
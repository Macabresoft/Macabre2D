namespace Macabre2D.UI.ViewModels.Dialogs {

    using Macabre2D.UI.Models;

    public sealed class SelectAssetViewModel : OKCancelDialogViewModel {
        private Asset _selectedAsset;

        public SelectAssetViewModel(Project project, AssetType assetMask, AssetType selectableAssetMask, bool allowNull) {
            this.Project = project;
            this.AssetMask = assetMask;
            this.SelectableAssetMask = selectableAssetMask;
            this.AllowNull = allowNull;
        }

        public bool AllowNull { get; }

        public AssetType AssetMask { get; }

        public Project Project { get; }

        public AssetType SelectableAssetMask { get; }

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
            return this.SelectedAsset is NullAsset || (this.SelectedAsset != null && this.SelectedAsset.Type != AssetType.Folder && (this.SelectedAsset.Type & this.SelectableAssetMask) == this.SelectedAsset.Type);
        }
    }
}
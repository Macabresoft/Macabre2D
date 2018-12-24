using Macabre2D.UI.Models;

namespace Macabre2D.UI.ViewModels.Dialogs {

    public sealed class SelectAssetViewModel : OKCancelDialogViewModel {
        private Asset _selectedAsset;

        public SelectAssetViewModel(Project project, AssetType assetMask, AssetType selectableAssetMask) {
            this.AssetMask = assetMask;
            this.Project = project;
            this.SelectableAssetMask = selectableAssetMask;
        }

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
            return this.SelectedAsset != null && this.SelectedAsset.Type != AssetType.Folder && (this.SelectedAsset.Type & this.SelectableAssetMask) == this.SelectedAsset.Type;
        }
    }
}
namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;
    using System.Windows;

    public interface IAssetService : INotifyPropertyChanged {
        Asset SelectedAsset { get; set; }

        void ChangeAssetParent(Asset assetToMove, FolderAsset newParent);

        DependencyObject GetEditor(Asset asset);

        void RenameAsset(Asset asset, string newName);
    }
}
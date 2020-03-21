namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    using Macabre2D.Engine.Windows.Models;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;

    public interface IAssetService : INotifyPropertyChanged {
        Asset SelectedAsset { get; set; }

        Task<bool> BuildAssets(BuildConfiguration configuration, BuildMode mode, params Asset[] assets);

        void ChangeAssetParent(Asset assetToMove, FolderAsset newParent);

        DependencyObject GetEditor(Asset asset);

        void RenameAsset(Asset asset, string newName);
    }
}
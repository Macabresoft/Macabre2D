namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.ComponentModel;
    using System.Windows;

    public interface IAssetService : INotifyPropertyChanged {
        Asset SelectedAsset { get; set; }

        DependencyObject GetEditor(Asset asset);
    }
}
namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models;
    using System.Windows;

    public interface IAssetEditorService {

        DependencyObject GetEditor(Asset asset);
    }
}
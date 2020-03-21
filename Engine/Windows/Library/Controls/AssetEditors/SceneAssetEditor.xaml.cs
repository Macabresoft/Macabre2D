namespace Macabre2D.UI.Controls.AssetEditors {

    using Macabre2D.UI.Models;
    using System.Windows;
    using System.Windows.Controls;

    public partial class SceneAssetEditor : UserControl {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(SceneAsset),
            typeof(SceneAssetEditor),
            new PropertyMetadata());

        public SceneAssetEditor() {
            this.InitializeComponent();
        }

        public SceneAsset Asset {
            get { return (SceneAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }
    }
}
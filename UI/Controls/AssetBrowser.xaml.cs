namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class AssetBrowser : UserControl {

        public static readonly DependencyProperty AssetDoubleClickedCommandProperty = DependencyProperty.Register(
            nameof(AssetDoubleClickedCommand),
            typeof(ICommand),
            typeof(AssetBrowser),
            new PropertyMetadata());

        public static readonly DependencyProperty AssetTypeMaskProperty = DependencyProperty.Register(
            nameof(AssetTypeMask),
            typeof(AssetType),
            typeof(AssetBrowser),
            new PropertyMetadata(AssetType.All));

        public static readonly DependencyProperty RootAssetProperty = DependencyProperty.Register(
            nameof(RootAsset),
            typeof(IParent<Asset>),
            typeof(AssetBrowser),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedAssetProperty = DependencyProperty.Register(
            nameof(SelectedAsset),
            typeof(Asset),
            typeof(AssetBrowser),
            new PropertyMetadata());

        public AssetBrowser() {
            this.InitializeComponent();

            this.Loaded += this.AssetBrowser_Loaded;
        }

        public ICommand AssetDoubleClickedCommand {
            get { return (ICommand)this.GetValue(AssetDoubleClickedCommandProperty); }
            set { this.SetValue(AssetDoubleClickedCommandProperty, value); }
        }

        public AssetType AssetTypeMask {
            get { return (AssetType)this.GetValue(AssetTypeMaskProperty); }
            set { this.SetValue(AssetTypeMaskProperty, value); }
        }

        public RelayCommand NavigateToAssetCommand { get; }

        public RelayCommand RefreshCommand { get; }

        public IParent<Asset> RootAsset {
            get { return (IParent<Asset>)this.GetValue(RootAssetProperty); }
            set { this.SetValue(RootAssetProperty, value); }
        }

        public Asset SelectedAsset {
            get { return (Asset)this.GetValue(SelectedAssetProperty); }
            set { this.SetValue(SelectedAssetProperty, value); }
        }

        private void AssetBrowser_Loaded(object sender, RoutedEventArgs e) {
            if (this._treeView.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem item) {
                item.IsSelected = true;
            }
        }

        private void OpenInFileExplorerMenuItem_Click(object sender, RoutedEventArgs e) {
            var directory = this.SelectedAsset.Type == AssetType.Folder ? this.SelectedAsset.GetPath() : new FileInfo(this.SelectedAsset.GetPath()).Directory.FullName;
            Process.Start(directory);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            this.SelectedAsset = e.NewValue as Asset;

            if (this.SelectedAsset.Type == AssetType.Folder) {
                // Set context menu to folder
            }
            else {
                // Set context menu to
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.OriginalSource is DependencyObject dependencyObject) {
                var treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();
                if (treeViewItem != null) {
                    treeViewItem.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}
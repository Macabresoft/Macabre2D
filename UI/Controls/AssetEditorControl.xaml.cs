namespace Macabre2D.UI.Controls {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public partial class AssetEditorControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(Asset),
            typeof(AssetEditorControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnAssetChanged)));

        private readonly IAssetEditorService _assetEditorService;
        private DependencyObject _editor = null;

        public AssetEditorControl() {
            this._assetEditorService = ViewContainer.Resolve<IAssetEditorService>();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Asset Asset {
            get { return (Asset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public DependencyObject Editor {
            get {
                return this._editor;
            }

            private set {
                this._editor = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Editor)));
            }
        }

        private static void OnAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is AssetEditorControl control) {
                control.Editor = control._assetEditorService.GetEditor(e.NewValue as Asset);
            }
        }
    }
}
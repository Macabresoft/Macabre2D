namespace Macabre2D.Engine.Windows.Controls {

    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public partial class AssetEditorControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(Asset),
            typeof(AssetEditorControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnAssetChanged)));

        private readonly IAssetService _assetService;
        private DependencyObject _editor = null;

        public AssetEditorControl() {
            this._assetService = ViewContainer.Resolve<IAssetService>();
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
                control.Editor = control._assetService.GetEditor(e.NewValue as Asset);
            }
        }
    }
}
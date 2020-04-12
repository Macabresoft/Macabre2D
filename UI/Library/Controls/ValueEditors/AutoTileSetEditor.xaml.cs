namespace Macabre2D.UI.Library.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Services;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class AutoTileSetEditor : NamedValueEditor<AutoTileSet> {
        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private AutoTileSetAsset _asset;

        public AutoTileSetEditor() : base() {
            this.SelectAutoTileSetCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(AutoTileSetAsset), true, out var asset)) {
                    this.Asset = asset as AutoTileSetAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.Loaded += this.AutoTileSetEditor_Loaded;
            this.InitializeComponent();
        }

        public AutoTileSetAsset Asset {
            get {
                return this._asset;
            }

            set {
                this._asset = value;
                this.RaisePropertyChanged(nameof(this.Asset));
            }
        }

        public ICommand SelectAutoTileSetCommand { get; }

        private void AutoTileSetEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var autoTileSetAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<AutoTileSetAsset>();
                this.Asset = autoTileSetAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= AutoTileSetEditor_Loaded;
        }
    }
}
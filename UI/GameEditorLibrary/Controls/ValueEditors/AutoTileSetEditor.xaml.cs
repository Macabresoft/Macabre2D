namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class AutoTileSetEditor : NamedValueEditor<AutoTileSet> {
        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private AutoTileSetAsset _asset;

        public AutoTileSetEditor() : base() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(AutoTileSetAsset), true, out var asset)) {
                    this.Asset = asset as AutoTileSetAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

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

        public ICommand ClearCommand { get; }

        public ICommand SelectCommand { get; }

        private void AutoTileSetEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var autoTileSetAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<AutoTileSetAsset>();
                this.Asset = autoTileSetAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= AutoTileSetEditor_Loaded;
        }
    }
}
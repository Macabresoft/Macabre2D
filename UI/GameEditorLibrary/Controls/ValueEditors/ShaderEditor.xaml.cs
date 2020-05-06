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

    public partial class ShaderEditor : NamedValueEditor<Shader> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(ShaderAsset),
            typeof(ShaderEditor),
            new PropertyMetadata());

        private readonly IGameDialogService _dialogService = ViewContainer.Resolve<IGameDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public ShaderEditor() : base() {
            this.SelectCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(ShaderAsset), true, out var asset)) {
                    this.Asset = asset as ShaderAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.ClearCommand = new RelayCommand(() => {
                this.Value = null;
            });

            this.Loaded += this.ShaderEditor_Loaded;
            this.InitializeComponent();
        }

        public ShaderAsset Asset {
            get { return (ShaderAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand ClearCommand { get; }
        public ICommand SelectCommand { get; }

        private void ShaderEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var shaderAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<ShaderAsset>();
                this.Asset = shaderAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= this.ShaderEditor_Loaded;
        }
    }
}
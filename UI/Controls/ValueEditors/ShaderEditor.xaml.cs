using GalaSoft.MvvmLight.CommandWpf;
using Macabre2D.Framework;
using Macabre2D.UI.Common;
using Macabre2D.UI.Models;
using Macabre2D.UI.ServiceInterfaces;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Macabre2D.UI.Controls.ValueEditors {

    public partial class ShaderEditor : NamedValueEditor<Shader> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(ShaderAsset),
            typeof(ShaderEditor),
            new PropertyMetadata());

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public ShaderEditor() {
            this.SelectShaderCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(ShaderAsset), true, out var asset)) {
                    this.Asset = asset as ShaderAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.Loaded += ShaderEditor_Loaded;
            this.InitializeComponent();
        }

        public ShaderAsset Asset {
            get { return (ShaderAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand SelectShaderCommand { get; }

        private void ShaderEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var shaderAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<ShaderAsset>();
                this.Asset = shaderAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= this.ShaderEditor_Loaded;
        }
    }
}
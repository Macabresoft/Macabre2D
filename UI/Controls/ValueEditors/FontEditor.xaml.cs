namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class FontEditor : NamedValueEditor<Font> {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(FontAsset),
            typeof(FontEditor),
            new PropertyMetadata());

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();
        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        public FontEditor() {
            this.SelectFontCommand = new RelayCommand(() => {
                if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, typeof(FontAsset), true, out var asset)) {
                    this.Asset = asset as FontAsset;
                    this.Value = this.Asset?.SavableValue;
                }
            }, true);

            this.Loaded += this.FontEditor_Loaded;
            this.InitializeComponent();
        }

        public FontAsset Asset {
            get { return (FontAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand SelectFontCommand { get; }

        private void FontEditor_Loaded(object sender, RoutedEventArgs e) {
            if (this.Value != null && (this.Asset == null || this.Asset.SavableValue?.Id != this.Value.Id)) {
                var fontAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<FontAsset>();
                this.Asset = fontAssets.FirstOrDefault(x => x.SavableValue.Id == this.Value.Id);
            }

            this.Loaded -= this.FontEditor_Loaded;
        }
    }
}
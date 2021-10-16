namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    public class SpriteSheetAssetSelectionDialog : BaseDialog {
        public SpriteSheetAssetSelectionDialog() {
        }

        [InjectionConstructor]
        public SpriteSheetAssetSelectionDialog(BaseDialogViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
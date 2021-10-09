namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    public class AutoTileSetEditorDialog : BaseDialog {
        public AutoTileSetEditorDialog() {
        }

        [InjectionConstructor]
        public AutoTileSetEditorDialog(AutoTileSetEditorViewModel viewModel) {
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
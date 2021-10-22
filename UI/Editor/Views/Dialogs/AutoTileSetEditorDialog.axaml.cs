namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    public class AutoTileSetEditorDialog : BaseDialog {
        public AutoTileSetEditorDialog() {
        }

        [InjectionConstructor]
        public AutoTileSetEditorDialog(AutoTileSetEditorViewModel viewModel) {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
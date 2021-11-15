namespace Macabresoft.Macabre2D.UI.Common {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Unity;

    public class AutoTileSetEditorView : UserControl {
        public AutoTileSetEditorView() {
        }

        [InjectionConstructor]
        public AutoTileSetEditorView(AutoTileSetEditorViewModel viewModel) {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
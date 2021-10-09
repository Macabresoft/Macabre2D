namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    public class SpriteSelectionDialog : BaseDialog {
        public SpriteSelectionDialog() {
        }

        [InjectionConstructor]
        public SpriteSelectionDialog(SpriteSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public SpriteSelectionViewModel ViewModel => this.DataContext as SpriteSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
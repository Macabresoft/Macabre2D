namespace Macabresoft.Macabre2D.UI.Common {
    using Avalonia.Markup.Xaml;
    using Unity;

    public class ContentSelectionDialog : BaseDialog {
        public ContentSelectionDialog() {
        }

        [InjectionConstructor]
        public ContentSelectionDialog(ContentSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public ContentSelectionViewModel ViewModel => this.DataContext as ContentSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
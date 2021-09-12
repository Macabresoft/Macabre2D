namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class AutoTileSetSelectionDialog : BaseDialog {
        public AutoTileSetSelectionDialog() {
        }

        [InjectionConstructor]
        public AutoTileSetSelectionDialog(AutoTileSetSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public AutoTileSetSelectionViewModel ViewModel => this.DataContext as AutoTileSetSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using Unity;

    public class AssetSelectionDialog : Window {
        
        public AssetSelectionDialog() {
        }
        
        [InjectionConstructor]
        public AssetSelectionDialog(AssetSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += OnCloseRequested;
            this.InitializeComponent();
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }

        public AssetSelectionViewModel ViewModel => this.DataContext as AssetSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
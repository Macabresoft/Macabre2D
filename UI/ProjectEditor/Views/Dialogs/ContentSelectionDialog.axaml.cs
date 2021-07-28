namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class ContentSelectionDialog : Window {
        
        public ContentSelectionDialog() {
        }
        
        [InjectionConstructor]
        public ContentSelectionDialog(ContentSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += OnCloseRequested;
            this.InitializeComponent();
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }

        public ContentSelectionViewModel ViewModel => this.DataContext as ContentSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
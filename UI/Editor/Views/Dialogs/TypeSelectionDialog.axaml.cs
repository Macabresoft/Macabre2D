namespace Macabresoft.Macabre2D.UI.Editor.Views.Dialogs {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    public class TypeSelectionDialog : BaseDialog {
        public TypeSelectionDialog() {
        }

        [InjectionConstructor]
        public TypeSelectionDialog(TypeSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public TypeSelectionViewModel ViewModel => this.DataContext as TypeSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
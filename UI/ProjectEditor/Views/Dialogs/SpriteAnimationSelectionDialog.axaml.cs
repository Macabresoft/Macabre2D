namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class SpriteAnimationSelectionDialog : BaseDialog {
        public SpriteAnimationSelectionDialog() {
        }

        [InjectionConstructor]
        public SpriteAnimationSelectionDialog(SpriteAnimationSelectionViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public SpriteAnimationSelectionViewModel ViewModel => this.DataContext as SpriteAnimationSelectionViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }
    }
}
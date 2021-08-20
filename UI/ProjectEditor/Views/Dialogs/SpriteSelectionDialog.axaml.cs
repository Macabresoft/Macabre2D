namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using System;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class SpriteSelectionDialog : Window {
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
namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using System.ComponentModel;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;

    public class MainWindow : Window {
        private bool _shouldClose = false;
        
        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }
        
        public MainWindowViewModel ViewModel => this.DataContext as MainWindowViewModel;

        protected override async void OnClosing(CancelEventArgs e) {
            if (!this._shouldClose && this.ViewModel is MainWindowViewModel viewModel) {
                e.Cancel = true;
                // ASYNC doesn't seem to be working here. The window pops up if I switch to .Result, but then its just fuckin frozen
                if (await viewModel.ShouldClose()) {
                    this._shouldClose = true;
                    this.Close();
                }
            }
            
            base.OnClosing(e);
        }
    }
}
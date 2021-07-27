namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using System.ComponentModel;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;

    public class MainWindow : Window {
        private readonly IEditorSettingsService _settingsService;
        private bool _shouldClose;
        
        public MainWindowViewModel ViewModel => this.DataContext as MainWindowViewModel;

        protected override async void OnClosing(CancelEventArgs e) {
            if (!this._shouldClose && this.ViewModel is MainWindowViewModel viewModel) {
                e.Cancel = true;

                if (await viewModel.TryClose() != YesNoCancelResult.Cancel) {
                    this._shouldClose = true;
                    this.Close();
                }
            }

            base.OnClosing(e);
        }

        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }
    }
}
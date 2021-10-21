namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class MainWindow : Window, IWindow {
        public MainWindowViewModel ViewModel => this.DataContext as MainWindowViewModel;

        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }

        private void TitleBar_OnDoubleTapped(object sender, RoutedEventArgs e) {
            if (this.ViewModel is MainWindowViewModel viewModel && viewModel.ToggleWindowStateCommand.CanExecute(this)) {
                viewModel.ToggleWindowStateCommand.Execute(this);
            }
        }

        private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            this.BeginMoveDrag(e);
        }
    }
}
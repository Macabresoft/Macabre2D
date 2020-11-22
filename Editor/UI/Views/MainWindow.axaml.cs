namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;

    public class MainWindow : Window {
        public MainWindow() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            this.InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
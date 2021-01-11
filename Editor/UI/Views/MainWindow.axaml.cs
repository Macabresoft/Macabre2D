namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;

    public class MainWindow : Window {
        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }

        private void Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void ViewSource_Click(object sender, RoutedEventArgs e) {
            WebHelper.OpenInBrowser("https://github.com/Macabresoft/Macabre2D");
        }
    }
}
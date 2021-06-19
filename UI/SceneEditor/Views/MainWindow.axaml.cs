namespace Macabresoft.Macabre2D.UI.SceneEditor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Library.ViewModels;

    public class MainWindow : Window {
        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }
    }
}
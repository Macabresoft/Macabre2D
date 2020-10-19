namespace Macabresoft.Macabre2D.Editor.UI.Views {

    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window {

        public MainWindow() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
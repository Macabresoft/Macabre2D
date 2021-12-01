namespace Macabresoft.Macabre2D.UI.Editor.Views.Dialogs {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class SplashScreen : Window {
        public SplashScreen() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
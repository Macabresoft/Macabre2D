namespace Macabresoft.MonoGame.Samples.AvaloniaWindow {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.MonoGame.AvaloniaUI;

    public class MainWindow : Window {

        public MainWindow() {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public IMonoGameViewModel MonoGameViewModel { get; } = new SampleMonoGameViewModel();

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {

    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.AvaloniaUI;

    public class MainWindow : Window {

        public MainWindow() {
            this.InitializeComponent();
        }

        public IMonoGameViewModel MonoGameViewModel { get; } = new SampleMonoGameViewModel();

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
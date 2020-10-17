namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {

    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window {

        public MainWindow() {
            this.InitializeComponent();
        }

        public SampleMonoGameViewModel ViewModel { get; } = new SampleMonoGameViewModel();

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
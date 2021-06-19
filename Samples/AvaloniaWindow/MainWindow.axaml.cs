namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;

    public class MainWindow : Window {
        public MainWindow() {
            this.SkullViewModel = new SkullViewModel(new AvaloniaGame());
            this.SolidViewModel = new SolidViewModel(this.SkullViewModel.Game);
            this.InitializeComponent();
        }

        public SkullViewModel SkullViewModel { get; }

        public SolidViewModel SolidViewModel { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
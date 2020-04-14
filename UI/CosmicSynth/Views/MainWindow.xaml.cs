namespace Macabre2D.UI.CosmicSynth.Views {

    using Macabre2D.UI.CosmicSynth.ViewModels;
    using System.Windows;

    public partial class MainWindow {

        public MainWindow(MainWindowViewModel viewModel) {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void MenuItem_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            // We don't want double clicking a menu item to cause the title bar double click event.
            e.Handled = true;
        }
    }
}
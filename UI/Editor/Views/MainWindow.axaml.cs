namespace Macabresoft.Macabre2D.UI.Editor {
    using System.ComponentModel;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class MainWindow : Window, IWindow {
        public static readonly DirectProperty<MainWindow, bool> ShowMaximizeProperty =
            AvaloniaProperty.RegisterDirect<MainWindow, bool>(
                nameof(ShowMaximize),
                editor => editor.ShowMaximize);
        
        public bool ShowMaximize => this.WindowState is WindowState.Normal or WindowState.Minimized;

        public MainWindowViewModel ViewModel => this.DataContext as MainWindowViewModel;
        
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.WindowState)) {
                this.RaisePropertyChanged(ShowMaximizeProperty, Optional<bool>.Empty, new BindingValue<bool>(this.ShowMaximize));
            }
        }

        internal void InitializeComponent() {
            this.DataContext = Resolver.Resolve<MainWindowViewModel>();
            AvaloniaXamlLoader.Load(this);
        }

        private void TitleBar_OnDoubleTapped(object sender, RoutedEventArgs e) {
            if (this.ViewModel is MainWindowViewModel viewModel && viewModel.ToggleWindowStateCommand.CanExecute(this)) {
                viewModel.ToggleWindowStateCommand.Execute(this);
            }
        }

        private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            this.BeginMoveDrag(e);
        }
    }
}
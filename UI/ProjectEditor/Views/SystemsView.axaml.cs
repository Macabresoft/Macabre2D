namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;

    public class SystemsView : UserControl {
        public SystemsView() {
            this.DataContext = Resolver.Resolve<SystemsViewModel>();
            this.InitializeComponent();
        }
        
        public SystemsViewModel ViewModel => this.DataContext as SystemsViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Systems {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Systems;

    public class SystemsView : UserControl {
        public SystemsView() {
            this.DataContext = Resolver.Resolve<SystemsViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
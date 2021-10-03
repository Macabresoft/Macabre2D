namespace Macabresoft.Macabre2D.UI.Editor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ProjectView : UserControl {
        public ProjectView() {
            this.DataContext = Resolver.Resolve<ProjectViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
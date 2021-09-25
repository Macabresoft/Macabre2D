namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;

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
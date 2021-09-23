namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class ProjectView : UserControl {
        public ProjectView() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
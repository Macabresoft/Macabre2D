namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ProjectEditorView : UserControl {
        public ProjectEditorView() {
            this.ViewModel = Resolver.Resolve<ProjectEditorViewModel>();
            this.InitializeComponent();
        }

        public ProjectEditorViewModel ViewModel { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
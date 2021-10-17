namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class ProjectEditorView : UserControl {
        public ProjectEditorView() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
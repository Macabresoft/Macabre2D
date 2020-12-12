namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using Avalonia.Markup.Xaml;

    public class StringEditor : ValueEditorControl<string> {
        public StringEditor() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
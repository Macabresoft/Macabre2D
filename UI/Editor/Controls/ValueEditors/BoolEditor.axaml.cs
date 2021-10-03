namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueEditors {
    using Avalonia.Markup.Xaml;

    public class BoolEditor : ValueEditorControl<bool> {
        public BoolEditor() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
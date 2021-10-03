namespace Macabresoft.Macabre2D.UI.Editor {
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
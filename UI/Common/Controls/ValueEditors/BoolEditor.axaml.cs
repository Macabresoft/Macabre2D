namespace Macabresoft.Macabre2D.UI.Common {
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
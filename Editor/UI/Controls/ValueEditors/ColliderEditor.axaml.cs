namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;

    public class ColliderEditor : ValueEditorControl<Collider> {
        public ColliderEditor() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
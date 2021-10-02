namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Unity;

    public class CollisionMapEditor : ValueEditorControl<CollisionMap> {
        [InjectionConstructor]
        public CollisionMapEditor() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
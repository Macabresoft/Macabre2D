namespace Macabresoft.Macabre2D.UI.SceneEditor.Views.Scene {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;

    public class EntityEditorView : UserControl {
        public EntityEditorView() {
            this.DataContext = Resolver.Resolve<EntityEditorViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
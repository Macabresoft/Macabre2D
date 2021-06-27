namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Scene {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;

    public class SceneEditorView : UserControl {
        public SceneEditorView() {
            this.DataContext = Resolver.Resolve<SceneEditorViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
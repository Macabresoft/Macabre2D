namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels;

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
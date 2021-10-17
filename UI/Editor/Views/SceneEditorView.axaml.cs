namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class SceneEditorView : UserControl {
        public SceneEditorView() {
            this.ViewModel = Resolver.Resolve<SceneEditorViewModel>();
            this.InitializeComponent();
        }
        
        public SceneEditorViewModel ViewModel { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
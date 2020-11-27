namespace Macabresoft.Macabre2D.Editor.UI.Views {
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
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
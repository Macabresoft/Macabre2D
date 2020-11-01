using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Macabresoft.Macabresoft.Macabre2D.Editor.UI.Views {
    public class SceneEditorView : UserControl {
        public SceneEditorView() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

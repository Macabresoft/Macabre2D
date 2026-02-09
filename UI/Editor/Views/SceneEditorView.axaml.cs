namespace Macabre2D.UI.Editor;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabre2D.UI.Common;

public partial class SceneEditorView : UserControl {
    public SceneEditorView() {
        this.ViewModel = Resolver.Resolve<SceneEditorViewModel>();
        this.InitializeComponent();
    }

    public SceneEditorViewModel ViewModel { get; }
}
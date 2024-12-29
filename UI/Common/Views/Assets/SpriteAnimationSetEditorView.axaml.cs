namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Unity;

public partial class SpriteAnimationSetEditorView : UserControl {
    [InjectionConstructor]
    public SpriteAnimationSetEditorView(SpriteAnimationSetEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
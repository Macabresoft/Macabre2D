namespace Macabre2D.UI.Common;

using Avalonia.Controls;
using Unity;

public partial class SpriteSheetIconSetEditorView : UserControl {
    [InjectionConstructor]
    public SpriteSheetIconSetEditorView(SpriteSheetIconSetEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
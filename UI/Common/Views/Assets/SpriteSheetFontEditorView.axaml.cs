namespace Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Unity;

public partial class SpriteSheetFontEditorView : UserControl {
    public SpriteSheetFontEditorView() {
    }

    [InjectionConstructor]
    public SpriteSheetFontEditorView(SpriteSheetFontEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
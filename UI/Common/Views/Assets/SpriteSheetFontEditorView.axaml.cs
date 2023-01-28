namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Unity;

public class SpriteSheetFontEditorView : UserControl {
    public SpriteSheetFontEditorView() {
    }

    [InjectionConstructor]
    public SpriteSheetFontEditorView(SpriteSheetFontEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
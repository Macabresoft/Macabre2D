namespace Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Unity;

public partial class SpriteSheetFontEditorView : UserControl {
    public SpriteSheetFontEditorView() {
    }

    [InjectionConstructor]
    public SpriteSheetFontEditorView(SpriteSheetFontEditorViewModel viewModel) {
        this.ViewModel = viewModel;
        this.InitializeComponent();
    }

    public SpriteSheetFontEditorViewModel ViewModel {
        get;
        private set {
            field = value;
            this.DataContext = value;
        }
    }
}
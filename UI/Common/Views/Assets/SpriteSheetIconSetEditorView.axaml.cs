namespace Macabre2D.UI.Common;

using Avalonia.Controls;
using Unity;

public partial class SpriteSheetIconSetEditorView : UserControl {
    [InjectionConstructor]
    public SpriteSheetIconSetEditorView(SpriteSheetIconSetEditorViewModel viewModel) {
        this.ViewModel = viewModel;
        this.InitializeComponent();
    }

    public SpriteSheetIconSetEditorViewModel ViewModel {
        get;
        private set {
            field = value;
            this.DataContext = value;
        }
    }
}
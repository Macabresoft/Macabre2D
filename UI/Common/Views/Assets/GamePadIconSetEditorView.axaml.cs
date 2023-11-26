namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Unity;

public partial class GamePadIconSetEditorView : UserControl {
    [InjectionConstructor]
    public GamePadIconSetEditorView(GamePadIconSetEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
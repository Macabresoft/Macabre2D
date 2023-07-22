namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Unity;

public partial class AutoTileSetEditorView : UserControl {
    public AutoTileSetEditorView() {
    }

    [InjectionConstructor]
    public AutoTileSetEditorView(AutoTileSetEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Unity;

public partial class KeyboardIconSetEditorView : UserControl {
    [InjectionConstructor]
    public KeyboardIconSetEditorView(KeyboardIconSetEditorViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
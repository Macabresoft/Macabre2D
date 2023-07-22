namespace Macabresoft.Macabre2D.UI.Editor;

using Macabresoft.AvaloniaEx;
using Unity;

public partial class SpriteSelectionDialog : BaseDialog {
    public SpriteSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSelectionDialog(SpriteSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public SpriteSelectionViewModel ViewModel => this.DataContext as SpriteSelectionViewModel;
}
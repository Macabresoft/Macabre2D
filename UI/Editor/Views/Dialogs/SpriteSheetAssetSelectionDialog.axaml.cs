namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class SpriteSheetAssetSelectionDialog : BaseDialog {
    public SpriteSheetAssetSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSheetAssetSelectionDialog(BaseDialogViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
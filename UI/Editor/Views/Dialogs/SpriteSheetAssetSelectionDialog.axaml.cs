namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public class SpriteSheetAssetSelectionDialog : BaseDialog {
    public SpriteSheetAssetSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSheetAssetSelectionDialog(BaseDialogViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
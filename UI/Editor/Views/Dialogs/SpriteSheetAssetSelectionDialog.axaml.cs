namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Input;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public partial class SpriteSheetAssetSelectionDialog : BaseDialog {
    public SpriteSheetAssetSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSheetAssetSelectionDialog(BaseDialogViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public FilterableViewModel<FilteredContentWrapper> ViewModel => this.DataContext as FilterableViewModel<FilteredContentWrapper>;

    private void FilteredNode_OnDoubleTapped(object sender, TappedEventArgs e) {
        this.ViewModel.ClearFilterCommand.Execute(null);
    }
}
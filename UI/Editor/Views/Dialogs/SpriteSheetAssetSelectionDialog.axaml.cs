namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (this.Find<IInputElement>("_filterBox") is { } filterBox) {
            filterBox.Focus();
        }
    }

    private void FilteredNode_OnDoubleTapped(object sender, TappedEventArgs e) {
        this.ViewModel.ClearFilterCommand.Execute(null);
    }
}
namespace Macabre2D.UI.Editor;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Macabresoft.AvaloniaEx;
using Macabre2D.UI.Common;
using Unity;

public partial class SpriteSheetAssetSelectionDialog : BaseDialog {
    public SpriteSheetAssetSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSheetAssetSelectionDialog(SpriteSheetAssetSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public SpriteSheetAssetSelectionViewModel ViewModel => this.DataContext as SpriteSheetAssetSelectionViewModel;

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
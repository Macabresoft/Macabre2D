namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
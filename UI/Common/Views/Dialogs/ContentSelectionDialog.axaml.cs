namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class ContentSelectionDialog : BaseDialog {
    public ContentSelectionDialog() {
    }

    [InjectionConstructor]
    public ContentSelectionDialog(ContentSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public ContentSelectionViewModel ViewModel => this.DataContext as ContentSelectionViewModel;

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
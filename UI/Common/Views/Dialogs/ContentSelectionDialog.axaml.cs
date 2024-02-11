namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Input;
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

    private void FilteredNode_OnDoubleTapped(object sender, TappedEventArgs e) {
        this.ViewModel.ClearFilterCommand.Execute(null);
    }
}
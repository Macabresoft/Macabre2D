namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
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
}
namespace Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class SystemSelectionDialog : BaseDialog {
    public SystemSelectionDialog() {
    }

    [InjectionConstructor]
    public SystemSelectionDialog(SystemSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public SystemSelectionViewModel ViewModel => this.DataContext as SystemSelectionViewModel;
}
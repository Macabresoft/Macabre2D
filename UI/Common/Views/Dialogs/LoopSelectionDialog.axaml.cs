namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public partial class LoopSelectionDialog : BaseDialog {
    public LoopSelectionDialog() {
    }

    [InjectionConstructor]
    public LoopSelectionDialog(LoopSelectionViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public LoopSelectionViewModel ViewModel => this.DataContext as LoopSelectionViewModel;
}
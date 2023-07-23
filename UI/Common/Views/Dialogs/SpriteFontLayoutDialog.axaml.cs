namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Unity;

public partial class SpriteFontLayoutDialog : BaseDialog {
    public SpriteFontLayoutDialog() : base() {
    }

    [InjectionConstructor]
    public SpriteFontLayoutDialog(SpriteFontLayoutViewModel viewModel) : base() {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public SpriteFontLayoutViewModel ViewModel => this.DataContext as SpriteFontLayoutViewModel;
}
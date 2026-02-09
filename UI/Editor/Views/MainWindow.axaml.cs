namespace Macabre2D.UI.Editor;

using Macabresoft.AvaloniaEx;
using Macabre2D.UI.Common;

public partial class MainWindow : BaseDialog {
    internal void Initialize() {
        this.InitializeComponent();
        this.DataContext = Resolver.Resolve<MainWindowViewModel>();
    }
}
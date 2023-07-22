namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.UI.Common;

public partial class MainWindow : BaseDialog {

    public MainWindow() : base() {
    }
    
    internal void Initialize() {
        this.DataContext = Resolver.Resolve<MainWindowViewModel>();
        this.InitializeComponent();
    }
}
namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.UI.Common;

public class MainWindow : BaseDialog {
    internal void InitializeComponent() {
        this.DataContext = Resolver.Resolve<MainWindowViewModel>();
        AvaloniaXamlLoader.Load(this);
    }
}
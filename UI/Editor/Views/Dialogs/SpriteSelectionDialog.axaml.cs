namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public class SpriteSelectionDialog : BaseDialog {
    public SpriteSelectionDialog() {
    }

    [InjectionConstructor]
    public SpriteSelectionDialog(SpriteSelectionViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public SpriteSelectionViewModel ViewModel => this.DataContext as SpriteSelectionViewModel;

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
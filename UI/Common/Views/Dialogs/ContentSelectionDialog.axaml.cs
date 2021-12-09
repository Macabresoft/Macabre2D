namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public class ContentSelectionDialog : BaseDialog {
    public ContentSelectionDialog() {
    }

    [InjectionConstructor]
    public ContentSelectionDialog(ContentSelectionViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public ContentSelectionViewModel ViewModel => this.DataContext as ContentSelectionViewModel;

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
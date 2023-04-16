namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public class EntitySelectionDialog : BaseDialog {
    public EntitySelectionDialog() {
    }

    [InjectionConstructor]
    public EntitySelectionDialog(EntitySelectionViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }

    public EntitySelectionViewModel ViewModel => this.DataContext as EntitySelectionViewModel;

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Unity;

public class TypeSelectionDialog : BaseDialog {
    public TypeSelectionDialog() : base() {
    }

    [InjectionConstructor]
    public TypeSelectionDialog(TypeSelectionViewModel viewModel) {
        this.DataContext = viewModel;
        this.InitializeComponent();
    }


    public TypeSelectionViewModel ViewModel => this.DataContext as TypeSelectionViewModel;

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
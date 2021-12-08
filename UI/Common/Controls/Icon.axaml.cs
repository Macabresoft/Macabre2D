namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public class Icon : ContentControl {
    public Icon() {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
namespace Macabresoft.AvaloniaEx;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public class BusyIndicator : UserControl {
    public BusyIndicator() {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
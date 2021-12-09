namespace Macabresoft.AvaloniaEx;

using Avalonia;
using Avalonia.Markup.Xaml;
using Unity;

public class WarningDialog : BaseDialog {
    public static readonly StyledProperty<string> WarningMessageProperty =
        AvaloniaProperty.Register<WarningDialog, string>(nameof(WarningMessage));


    [InjectionConstructor]
    public WarningDialog() {
        this.InitializeComponent();
    }

    public string WarningMessage {
        get => this.GetValue(WarningMessageProperty);
        set => this.SetValue(WarningMessageProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
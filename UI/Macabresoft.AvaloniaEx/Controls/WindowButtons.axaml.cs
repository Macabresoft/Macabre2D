namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

public class WindowButtons : UserControl {
    public static readonly StyledProperty<ICommand> CloseCommandProperty =
        AvaloniaProperty.Register<WindowButtons, ICommand>(
            nameof(CloseCommand),
            defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<ICommand> MinimizeCommandProperty =
        AvaloniaProperty.Register<WindowButtons, ICommand>(
            nameof(MinimizeCommand),
            defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<ICommand> ToggleWindowStateCommandProperty =
        AvaloniaProperty.Register<WindowButtons, ICommand>(
            nameof(ToggleWindowStateCommand),
            defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<IWindow> WindowProperty =
        AvaloniaProperty.Register<WindowButtons, IWindow>(
            nameof(Window),
            defaultBindingMode: BindingMode.OneWay);

    public WindowButtons() {
        this.InitializeComponent();
    }

    public ICommand CloseCommand {
        get => this.GetValue(CloseCommandProperty);
        set => this.SetValue(CloseCommandProperty, value);
    }

    public ICommand MinimizeCommand {
        get => this.GetValue(MinimizeCommandProperty);
        set => this.SetValue(MinimizeCommandProperty, value);
    }

    public ICommand ToggleWindowStateCommand {
        get => this.GetValue(ToggleWindowStateCommandProperty);
        set => this.SetValue(ToggleWindowStateCommandProperty, value);
    }

    public IWindow Window {
        get => this.GetValue(WindowProperty);
        set => this.SetValue(WindowProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;

public class BaseDialog : Window, IWindow {
    public static readonly StyledProperty<ICommand> CloseCommandProperty =
        AvaloniaProperty.Register<BaseDialog, ICommand>(nameof(CloseCommand), defaultBindingMode: BindingMode.OneWay, defaultValue: WindowHelper.CloseDialogCommand);

    public static readonly StyledProperty<object> ContentLeftOfMenuProperty =
        AvaloniaProperty.Register<BaseDialog, object>(nameof(ContentLeftOfMenu), defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<Menu> MenuProperty =
        AvaloniaProperty.Register<BaseDialog, Menu>(nameof(Menu), defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<bool> ShowMinimizeProperty =
        AvaloniaProperty.Register<BaseDialog, bool>(nameof(ShowMinimize), defaultBindingMode: BindingMode.OneWay, defaultValue: true);

    public static readonly StyledProperty<StreamGeometry> VectorIconProperty =
        AvaloniaProperty.Register<BaseDialog, StreamGeometry>(nameof(VectorIcon), defaultBindingMode: BindingMode.OneWay);

    public ICommand CloseCommand {
        get => this.GetValue(CloseCommandProperty);
        set => this.SetValue(CloseCommandProperty, value);
    }

    public object ContentLeftOfMenu {
        get => this.GetValue(ContentLeftOfMenuProperty);
        set => this.SetValue(ContentLeftOfMenuProperty, value);
    }

    public Menu Menu {
        get => this.GetValue(MenuProperty);
        set => this.SetValue(MenuProperty, value);
    }

    public bool ShowMinimize {
        get => this.GetValue(ShowMinimizeProperty);
        set => this.SetValue(ShowMinimizeProperty, value);
    }

    public StreamGeometry VectorIcon {
        get => this.GetValue(VectorIconProperty);
        set => this.SetValue(VectorIconProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        this.ResetWindowChrome();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e) {
        base.OnPointerCaptureLost(e);
        this.ResetWindowChrome();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name is nameof(this.WindowState) or nameof(this.ClientSize)) {
            this.ResetWindowChrome();
        }
    }

    private void ResetWindowChrome() {
        if (this.WindowState is WindowState.Maximized or WindowState.FullScreen) {
            this.SystemDecorations = SystemDecorations.BorderOnly;
            this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
            this.ExtendClientAreaToDecorationsHint = false;
        }
        else {
            this.SystemDecorations = SystemDecorations.None;
            this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            this.ExtendClientAreaToDecorationsHint = true;
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void TitleBar_OnDoubleTapped(object sender, RoutedEventArgs e) {
        if (this.CanResize) {
            WindowHelper.ToggleWindowStateCommand.Execute(this);
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        this.BeginMoveDrag(e);
    }
}
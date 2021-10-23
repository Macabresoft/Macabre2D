namespace Macabresoft.Macabre2D.UI.Common {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Media;

    public class BaseDialog : Window, IWindow {
        public static readonly StyledProperty<ICommand> CloseCommandProperty =
            AvaloniaProperty.Register<BaseDialog, ICommand>(nameof(CloseCommand), defaultBindingMode: BindingMode.OneWay, defaultValue: WindowHelper.CloseDialogCommand);

        public static readonly StyledProperty<object> LeftOfMenuTitleBarContentProperty =
            AvaloniaProperty.Register<BaseDialog, object>(nameof(LeftOfMenuTitleBarContent), defaultBindingMode: BindingMode.OneWay);

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

        public object LeftOfMenuTitleBarContent {
            get => this.GetValue(LeftOfMenuTitleBarContentProperty);
            set => this.SetValue(LeftOfMenuTitleBarContentProperty, value);
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

        private void TitleBar_OnDoubleTapped(object sender, RoutedEventArgs e) {
            if (WindowHelper.ToggleWindowStateCommand.CanExecute(this)) {
                WindowHelper.ToggleWindowStateCommand.Execute(this);
            }
        }

        private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            this.BeginMoveDrag(e);
        }
    }
}
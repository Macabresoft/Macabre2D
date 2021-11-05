namespace Macabresoft.Macabre2D.UI.Common {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Media;
    using Avalonia.Threading;

    public class BaseDialog : Window, IWindow {
        public static readonly DirectProperty<BaseDialog, bool> ApplyContentMarginProperty =
            AvaloniaProperty.RegisterDirect<BaseDialog, bool>(
                nameof(ApplyContentMargin),
                editor => editor.ApplyContentMargin);

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

        private bool _applyContentMargin;
        private bool _isDragging;

        public bool ApplyContentMargin {
            get => this._applyContentMargin;
            private set => this.SetAndRaise(ApplyContentMarginProperty, ref this._applyContentMargin, value);
        }

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
            this.ResetContentMargin();
        }

        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e) {
            base.OnPointerCaptureLost(e);
            this.ResetContentMargin();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name is nameof(this.WindowState) or nameof(this.ClientSize)) {
                this.ResetContentMargin();
            }
        }

        private void ResetContentMargin() {
            Dispatcher.UIThread.Post(() => this.ApplyContentMargin = this.WindowState == WindowState.Maximized);
        }

        private void TitleBar_OnDoubleTapped(object sender, RoutedEventArgs e) {
            if (this.CanResize && WindowHelper.ToggleWindowStateCommand.CanExecute(this)) {
                WindowHelper.ToggleWindowStateCommand.Execute(this);
            }
        }

        private void TitleBar_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            this.ApplyContentMargin = false;
            this.BeginMoveDrag(e);
        }
    }
}
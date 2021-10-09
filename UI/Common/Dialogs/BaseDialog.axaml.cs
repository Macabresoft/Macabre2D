namespace Macabresoft.Macabre2D.UI.Common {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Media;

    public class BaseDialog : Window {
        public static readonly StyledProperty<Menu> MenuProperty =
            AvaloniaProperty.Register<BaseDialog, Menu>(nameof(Menu), defaultBindingMode: BindingMode.OneWay);

        public static readonly StyledProperty<StreamGeometry> VectorIconProperty =
            AvaloniaProperty.Register<BaseDialog, StreamGeometry>(nameof(VectorIcon), defaultBindingMode: BindingMode.OneWay);

        public Menu Menu {
            get => this.GetValue(MenuProperty);
            set => this.SetValue(MenuProperty, value);
        }

        public StreamGeometry VectorIcon {
            get => this.GetValue(VectorIconProperty);
            set => this.SetValue(VectorIconProperty, value);
        }
    }
}
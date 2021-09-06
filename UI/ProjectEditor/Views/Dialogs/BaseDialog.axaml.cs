namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Media;

    public class BaseDialog : Window {
        public static readonly StyledProperty<StreamGeometry> VectorIconProperty = 
            AvaloniaProperty.Register<BaseDialog, StreamGeometry>(nameof(VectorIcon), defaultBindingMode: BindingMode.OneWay);

        public StreamGeometry VectorIcon {
            get => this.GetValue(VectorIconProperty);
            set => this.SetValue(VectorIconProperty, value);
        }
    }
}
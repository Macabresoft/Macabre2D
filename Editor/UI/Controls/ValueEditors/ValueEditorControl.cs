namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using Avalonia;
    using Avalonia.Controls;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;

    public abstract class ValueEditorControl<T> : UserControl, IValueEditor<T> {
        public static readonly StyledProperty<object> OwnerProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, object>(nameof(Owner));

        public static readonly StyledProperty<T> ValueProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, T>(nameof(Value));

        public static readonly StyledProperty<string> ValuePropertyNameProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(ValuePropertyName));

        public object Owner {
            get => this.GetValue(OwnerProperty);
            set => this.SetValue(OwnerProperty, value);
        }

        public T Value {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public string ValuePropertyName {
            get => this.GetValue(ValuePropertyNameProperty);
            set => this.SetValue(ValuePropertyNameProperty, value);
        }
    }
}
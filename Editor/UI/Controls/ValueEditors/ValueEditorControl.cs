namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;

    public abstract class ValueEditorControl<T> : UserControl, IValueEditor<T> {
        public static readonly StyledProperty<object> OwnerProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, object>(nameof(Owner));

        public static readonly StyledProperty<T> ValueProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, T>(nameof(Value), notifying: OnValueChanging);

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(Title));


        public static readonly StyledProperty<string> ValuePropertyNameProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(ValuePropertyName));
        
        public object Owner {
            get => this.GetValue(OwnerProperty);
            set => this.SetValue(OwnerProperty, value);
        }

        public string Title {
            get => this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public T Value {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public string ValuePropertyName {
            get => this.GetValue(ValuePropertyNameProperty);
            set => this.SetValue(ValuePropertyNameProperty, value);
        }

        private void OnValueChanged(T originalValue, T updatedValue) {
            this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<T>(originalValue, updatedValue));
        }

        private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is ValueEditorControl<T> valueEditor &&
                valueEditor.Owner != null &&
                !string.IsNullOrWhiteSpace(valueEditor.ValuePropertyName)) {
                var originalValue = valueEditor.Owner.GetPropertyValue(valueEditor.ValuePropertyName);

                if (originalValue is T value) {
                    valueEditor.OnValueChanged(value, valueEditor.Value);
                }
            }
        }

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
    }
}
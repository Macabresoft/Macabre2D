namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;

    public abstract class ValueEditorControl<T> : UserControl, IValueEditor<T> {
        public static readonly StyledProperty<object> OwnerProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, object>(nameof(Owner));

        public static readonly StyledProperty<T> ValueProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, T>(nameof(Value), notifying: OnValueChanging, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(Title));

        public static readonly StyledProperty<bool> UpdateOnLostFocusProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, bool>(nameof(UpdateOnLostFocus), true);


        public static readonly StyledProperty<string> ValuePropertyNameProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(ValuePropertyName));

        public static readonly StyledProperty<Type> ValueTypeProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, Type>(nameof(ValueType));

        public event EventHandler<ValueChangedEventArgs<object>> ValueChanged;

        public object Owner {
            get => this.GetValue(OwnerProperty);
            set => this.SetValue(OwnerProperty, value);
        }

        public string Title {
            get => this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public bool UpdateOnLostFocus {
            get => this.GetValue(UpdateOnLostFocusProperty);
            set => this.SetValue(UpdateOnLostFocusProperty, value);
        }

        public T Value {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public string ValuePropertyName {
            get => this.GetValue(ValuePropertyNameProperty);
            set => this.SetValue(ValuePropertyNameProperty, value);
        }

        public Type ValueType {
            get => this.GetValue(ValueTypeProperty);
            set => this.SetValue(ValueTypeProperty, value);
        }

        public void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            if (value is T typedValue) {
                this.Value = typedValue;
            }

            this.Owner = owner;
            this.ValuePropertyName = valuePropertyName;
            this.Title = title;
        }

        public void SetValue(object newValue) {
            if (newValue is T typedNewValue) {
                this.Value = typedNewValue;
            }
        }

        protected virtual void OnValueChanged(T updatedValue) {
            if (this.Owner != null && !string.IsNullOrEmpty(this.ValuePropertyName)) {
                var originalValue = this.Owner.GetPropertyValue(this.ValuePropertyName);
                this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<object>(originalValue, updatedValue));
            }
        }

        protected void SetValue(T originalValue, T updatedValue) {
            this.Value = updatedValue;
            this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<object>(originalValue, updatedValue));
        }

        private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is ValueEditorControl<T> valueEditor) {
                valueEditor.OnValueChanged(valueEditor.Value);
            }
        }
    }
}
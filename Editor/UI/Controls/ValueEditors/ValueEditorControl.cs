namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Interactivity;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;

    public abstract class ValueEditorControl<T> : UserControl, IValueEditor<T> {
        public static readonly StyledProperty<object> OwnerProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, object>(nameof(Owner));

        public static readonly StyledProperty<T> ValueProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, T>(nameof(Value), notifying: OnValueChanging, defaultBindingMode: BindingMode.TwoWay);

        public static readonly DirectProperty<ValueEditorControl<T>, T> IntermediaryValueProperty =
            AvaloniaProperty.RegisterDirect<ValueEditorControl<T>, T>(
                nameof(IntermediaryValue),
                editor => editor.IntermediaryValue,
                (editor, value) => editor.IntermediaryValue = value);


        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(Title));

        public static readonly StyledProperty<bool> UpdateOnLostFocusProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, bool>(nameof(UpdateOnLostFocus), true);

        public static readonly StyledProperty<string> ValuePropertyNameProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, string>(nameof(ValuePropertyName));

        private T _intermediaryValue;

        public T IntermediaryValue {
            get => this._intermediaryValue;
            set {
                if (!this.UpdateOnLostFocus) {
                    this.SetValue(this.Value, value);
                }

                this.SetAndRaise(IntermediaryValueProperty, ref this._intermediaryValue, value);
            }
        }

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

        private bool HasValueChanged() {
            if (this.Value != null) {
                return !this.Value.Equals(this.IntermediaryValue);
            }

            return this.IntermediaryValue != null;
        }

        private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is ValueEditorControl<T> valueEditor && valueEditor.HasValueChanged()) {
                valueEditor.SetAndRaise(IntermediaryValueProperty, ref valueEditor._intermediaryValue, valueEditor.Value);
            }
        }

        private void SetValue(T originalValue, T updatedValue) {
            this.Value = updatedValue;
            this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<T>(originalValue, updatedValue));
        }

        protected void ValueEditor_OnLostFocus(object sender, RoutedEventArgs e) {
            if (this.UpdateOnLostFocus && this.HasValueChanged()) {
                this.SetValue(this.Value, this.IntermediaryValue);
            }
        }

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
    }
}
﻿namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Data;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.LogicalTree;

    public abstract class BaseNumericEditor<TNumeric> : ValueEditorControl<TNumeric> where TNumeric : struct, IComparable {
        public static readonly DirectProperty<BaseNumericEditor<TNumeric>, string> ValueDisplayProperty =
            AvaloniaProperty.RegisterDirect<BaseNumericEditor<TNumeric>, string>(
                nameof(ValueDisplay),
                editor => editor.ValueDisplay,
                (editor, value) => editor.ValueDisplay = value);
        
        public static readonly StyledProperty<TNumeric> ValueMaximumProperty =
            AvaloniaProperty.Register<BaseNumericEditor<TNumeric>, TNumeric>(nameof(ValueMaximum), GetMinValue());
        
        public static readonly StyledProperty<TNumeric> ValueMinimumProperty =
            AvaloniaProperty.Register<BaseNumericEditor<TNumeric>, TNumeric>(nameof(ValueMinimum), GetMaxValue());
        
        private readonly DataTable _calculator = new();
        private string _valueDisplay;

        public string ValueDisplay {
            get => this._valueDisplay;
            set => this.SetAndRaise(ValueDisplayProperty, ref this._valueDisplay, value);
        }

        public TNumeric ValueMaximum {
            get => this.GetValue(ValueMaximumProperty);
            set => this.SetValue(ValueMaximumProperty, value);
        }

        public TNumeric ValueMinimum {
            get => this.GetValue(ValueMinimumProperty);
            set => this.SetValue(ValueMinimumProperty, value);
        }

        protected abstract TNumeric ConvertValue(object calculatedValue);

        protected void InputElement_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                this.Value = this.GetCalculatedValue(this.ValueDisplay, this.Value);
            }
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
            base.OnAttachedToLogicalTree(e);
            this.UpdateDisplayValue();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.Value)) {
                this.UpdateDisplayValue();
            }
        }

        protected void ValueDisplay_OnLostFocus(object sender, RoutedEventArgs e) {
            this.Value = this.GetCalculatedValue(this.ValueDisplay, this.Value);
        }

        private TNumeric GetCalculatedValue(string expression, TNumeric fallBack) {
            var result = fallBack;
            var newValue = this._calculator.Compute(expression, null);
            try {
                var convertedValue = this.ConvertValue(newValue);
                result = convertedValue;
            }
            catch {
                // The user can type whatever they want into the display fields.
                // This is a fallback if that gibberish can't be computed and converted into a float.
            }

            return result;
        }

        private static TNumeric GetMaxValue() {
            object result = default(TNumeric);

            result = result switch {
                byte => byte.MaxValue,
                float => float.MaxValue,
                int => int.MaxValue,
                uint => uint.MaxValue,
                short => short.MaxValue,
                ushort => ushort.MaxValue,
                long => long.MaxValue,
                ulong => ulong.MaxValue,
                double => double.MaxValue,
                _ => result
            };

            return (TNumeric)result;
        }

        private static TNumeric GetMinValue() {
            object result = default(TNumeric);

            result = result switch {
                byte => byte.MinValue,
                float => float.MinValue,
                int => int.MinValue,
                uint => uint.MinValue,
                short => short.MinValue,
                ushort => ushort.MinValue,
                long => long.MinValue,
                ulong => ulong.MinValue,
                double => double.MinValue,
                _ => result
            };

            return (TNumeric)result;
        }

        private void UpdateDisplayValue() {
            this._valueDisplay = this.Value.ToString();
            this.RaisePropertyChanged(ValueDisplayProperty, Optional<string>.Empty, this._valueDisplay);
        }
    }
}
namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Data;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Interactivity;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;

    public class FloatEditor : ValueEditorControl<float> {
        public static readonly StyledProperty<float> ValueMaximumProperty =
            AvaloniaProperty.Register<FloatEditor, float>(nameof(ValueMaximum), float.MaxValue);

        public static readonly DirectProperty<FloatEditor, string> ValueDisplayProperty =
            AvaloniaProperty.RegisterDirect<FloatEditor, string>(
                nameof(ValueDisplay),
                editor => editor.ValueDisplay,
                (editor, value) => editor.ValueDisplay = value);

        public static readonly StyledProperty<float> ValueMinimumProperty =
            AvaloniaProperty.Register<FloatEditor, float>(nameof(ValueMinimum), float.MinValue);

        private readonly DataTable _calculator = new();
        private string _valueDisplay;

        public FloatEditor() {
            this.InitializeComponent();
        }

        public string ValueDisplay {
            get => this._valueDisplay;
            set => this.SetAndRaise(ValueDisplayProperty, ref this._valueDisplay, value);
        }

        public float ValueMaximum {
            get => this.GetValue(ValueMaximumProperty);
            set => this.SetValue(ValueMaximumProperty, value);
        }

        public float ValueMinimum {
            get => this.GetValue(ValueMinimumProperty);
            set => this.SetValue(ValueMinimumProperty, value);
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

        private float GetCalculatedValue(string expression, float fallBack) {
            var result = fallBack;
            var newValue = this._calculator.Compute(expression, null);
            try {
                var floatValue = Convert.ToSingle(newValue);
                result = floatValue;
            }
            catch {
                // The user can type whatever they want into the display fields.
                // This is a fallback if that gibberish can't be computed and converted into a float.
            }

            return result;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateDisplayValue() {
            this._valueDisplay = this.Value.ToString();
            this.RaisePropertyChanged(ValueDisplayProperty, Optional<string>.Empty, this._valueDisplay);
        }

        private void ValueDisplay_OnLostFocus(object sender, RoutedEventArgs e) {
            this.Value = this.GetCalculatedValue(this.ValueDisplay, this.Value);
        }
    }
}
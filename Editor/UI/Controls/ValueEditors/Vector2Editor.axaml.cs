namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Data;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Interactivity;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Microsoft.Xna.Framework;

    public class Vector2Editor : ValueEditorControl<Vector2> {
        public static readonly DirectProperty<Vector2Editor, string> XDisplayProperty =
            AvaloniaProperty.RegisterDirect<Vector2Editor, string>(
                nameof(XDisplay),
                editor => editor.XDisplay,
                (editor, value) => editor.XDisplay = value);

        public static readonly StyledProperty<float> XMaximumProperty =
            AvaloniaProperty.Register<Vector2Editor, float>(nameof(XMaximum), float.MaxValue);

        public static readonly StyledProperty<float> XMinimumProperty =
            AvaloniaProperty.Register<Vector2Editor, float>(nameof(XMinimum), float.MinValue);

        public static readonly DirectProperty<Vector2Editor, string> YDisplayProperty =
            AvaloniaProperty.RegisterDirect<Vector2Editor, string>(
                nameof(YDisplay),
                editor => editor.YDisplay,
                (editor, value) => editor.YDisplay = value);

        public static readonly StyledProperty<float> YMaximumProperty =
            AvaloniaProperty.Register<Vector2Editor, float>(nameof(YMaximum), float.MaxValue);

        public static readonly StyledProperty<float> YMinimumProperty =
            AvaloniaProperty.Register<Vector2Editor, float>(nameof(YMinimum), float.MinValue);

        private readonly DataTable _calculator = new();
        private string _xDisplay;
        private string _yDisplay;

        public Vector2Editor() {
            this.InitializeComponent();
        }

        public string XDisplay {
            get => this._xDisplay;
            set => this.SetAndRaise(XDisplayProperty, ref this._xDisplay, value);
        }


        public float XMaximum {
            get => this.GetValue(XMaximumProperty);
            set => this.SetValue(XMaximumProperty, value);
        }

        public float XMinimum {
            get => this.GetValue(XMinimumProperty);
            set => this.SetValue(XMinimumProperty, value);
        }

        public string YDisplay {
            get => this._yDisplay;
            set => this.SetAndRaise(YDisplayProperty, ref this._yDisplay, value);
        }

        public float YMaximum {
            get => this.GetValue(YMaximumProperty);
            set => this.SetValue(YMaximumProperty, value);
        }

        public float YMinimum {
            get => this.GetValue(YMinimumProperty);
            set => this.SetValue(YMinimumProperty, value);
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
            base.OnAttachedToLogicalTree(e);
            this.UpdateDisplayValues();
        }


        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.Value)) {
                this.UpdateDisplayValues();
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

        private void UpdateDisplayValues() {
            this._xDisplay = this.Value.X.ToString();
            this._yDisplay = this.Value.Y.ToString();
            this.RaisePropertyChanged(XDisplayProperty, Optional<string>.Empty, this._xDisplay);
            this.RaisePropertyChanged(YDisplayProperty, Optional<string>.Empty, this._yDisplay);
        }


        private void XDisplay_OnLostFocus(object sender, RoutedEventArgs e) {
            var calculatedValue = this.GetCalculatedValue(this.XDisplay, this.IntermediaryValue.X);
            this.IntermediaryValue = new Vector2(calculatedValue, this.IntermediaryValue.Y);
        }

        private void YDisplay_OnLostFocus(object sender, RoutedEventArgs e) {
            var calculatedValue = this.GetCalculatedValue(this.YDisplay, this.IntermediaryValue.Y);
            this.IntermediaryValue = new Vector2(this.IntermediaryValue.X, calculatedValue);
        }
    }
}
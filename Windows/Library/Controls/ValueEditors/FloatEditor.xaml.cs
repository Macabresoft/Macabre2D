namespace Macabre2D.Engine.Windows.Controls.ValueEditors {

    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class FloatEditor : NamedValueEditor<float> {

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue),
            typeof(float),
            typeof(FloatEditor),
            new PropertyMetadata(float.MaxValue));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue),
            typeof(float),
            typeof(FloatEditor),
            new PropertyMetadata(float.MinValue));

        public FloatEditor() : base() {
            this.InitializeComponent();
        }

        public float MaxValue {
            get { return (float)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        public float MinValue {
            get { return (float)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        public string ValueWrapper {
            get {
                return this.Value.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this.Value = MathHelper.Clamp(Convert.ToSingle(expression.Evaluate()), this.MinValue, this.MaxValue);
                }

                this.RaisePropertyChanged();
            }
        }

        protected override void OnValueChanged(float newValue, float oldValue, DependencyObject d) {
            if (d is FloatEditor editor) {
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.ValueWrapper));
        }
    }
}
namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.UI.Common;
    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class VectorEditor : NamedValueEditor<Vector2> {

        public static readonly DependencyProperty MaxXValueProperty = DependencyProperty.Register(
            nameof(MaxXValue),
            typeof(float),
            typeof(VectorEditor),
            new PropertyMetadata(float.MaxValue));

        public static readonly DependencyProperty MaxYValueProperty = DependencyProperty.Register(
            nameof(MaxYValue),
            typeof(float),
            typeof(VectorEditor),
            new PropertyMetadata(float.MaxValue));

        public static readonly DependencyProperty MinXValueProperty = DependencyProperty.Register(
            nameof(MinXValue),
            typeof(float),
            typeof(VectorEditor),
            new PropertyMetadata(float.MinValue));

        public static readonly DependencyProperty MinYValueProperty = DependencyProperty.Register(
            nameof(MinYValue),
            typeof(float),
            typeof(VectorEditor),
            new PropertyMetadata(float.MinValue));

        private float _x;
        private float _y;

        public VectorEditor() {
            this.InitializeComponent();
        }

        public float MaxXValue {
            get { return (float)this.GetValue(MaxXValueProperty); }
            set { this.SetValue(MaxXValueProperty, value); }
        }

        public float MaxYValue {
            get { return (float)this.GetValue(MaxYValueProperty); }
            set { this.SetValue(MaxYValueProperty, value); }
        }

        public float MinXValue {
            get { return (float)this.GetValue(MinXValueProperty); }
            set { this.SetValue(MinXValueProperty, value); }
        }

        public float MinYValue {
            get { return (float)this.GetValue(MinYValueProperty); }
            set { this.SetValue(MinYValueProperty, value); }
        }

        public string X {
            get {
                return this._x.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this._x = MathHelper.Clamp(Convert.ToSingle(expression.Evaluate()), this.MinXValue, this.MaxXValue);
                    this.Value = new Vector2(this._x, this._y);
                }

                this.RaisePropertyChanged(nameof(this.X));
            }
        }

        public string Y {
            get {
                return this._y.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this._y = MathHelper.Clamp(Convert.ToSingle(expression.Evaluate()), this.MinYValue, this.MaxYValue);
                    this.Value = new Vector2(this._x, this._y);
                }

                this.RaisePropertyChanged(nameof(this.Y));
            }
        }

        protected override void OnValueChanged(Vector2 newValue, Vector2 oldValue, DependencyObject d) {
            if (d is VectorEditor editor) {
                editor._x = newValue.X;
                editor._y = newValue.Y;
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private static void OnVectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is VectorEditor editor && e.NewValue is Vector2 vector) {
                editor._x = vector.X;
                editor._y = vector.Y;
                editor.RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.X));
            this.RaisePropertyChanged(nameof(this.Y));
        }
    }
}
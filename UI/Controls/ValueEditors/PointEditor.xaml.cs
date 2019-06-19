namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.UI.Common;
    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class PointEditor : NamedValueEditor<Microsoft.Xna.Framework.Point> {

        public static readonly DependencyProperty MaxXValueProperty = DependencyProperty.Register(
            nameof(MaxXValue),
            typeof(int),
            typeof(PointEditor),
            new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty MaxYValueProperty = DependencyProperty.Register(
            nameof(MaxYValue),
            typeof(int),
            typeof(PointEditor),
            new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty MinXValueProperty = DependencyProperty.Register(
            nameof(MinXValue),
            typeof(int),
            typeof(PointEditor),
            new PropertyMetadata(int.MinValue));

        public static readonly DependencyProperty MinYValueProperty = DependencyProperty.Register(
            nameof(MinYValue),
            typeof(int),
            typeof(PointEditor),
            new PropertyMetadata(int.MinValue));

        private int _x;
        private int _y;

        public PointEditor() {
            this.InitializeComponent();
        }

        public int MaxXValue {
            get { return (int)this.GetValue(MaxXValueProperty); }
            set { this.SetValue(MaxXValueProperty, value); }
        }

        public int MaxYValue {
            get { return (int)this.GetValue(MaxYValueProperty); }
            set { this.SetValue(MaxYValueProperty, value); }
        }

        public int MinXValue {
            get { return (int)this.GetValue(MinXValueProperty); }
            set { this.SetValue(MinXValueProperty, value); }
        }

        public int MinYValue {
            get { return (int)this.GetValue(MinYValueProperty); }
            set { this.SetValue(MinYValueProperty, value); }
        }

        public string X {
            get {
                return this._x.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this._x = MathHelper.Clamp(Convert.ToInt32(expression.Evaluate()), this.MinXValue, this.MaxXValue);
                    this.Value = new Microsoft.Xna.Framework.Point(this._x, this._y);
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
                    this._y = MathHelper.Clamp(Convert.ToInt32(expression.Evaluate()), this.MinYValue, this.MaxYValue);
                    this.Value = new Microsoft.Xna.Framework.Point(this._x, this._y);
                }

                this.RaisePropertyChanged(nameof(this.Y));
            }
        }

        protected override void OnValueChanged(Microsoft.Xna.Framework.Point newValue, Microsoft.Xna.Framework.Point oldValue, DependencyObject d) {
            if (d is PointEditor editor) {
                editor._x = newValue.X;
                editor._y = newValue.Y;
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.X));
            this.RaisePropertyChanged(nameof(this.Y));
        }
    }
}
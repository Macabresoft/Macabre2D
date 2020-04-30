namespace Macabre2D.UI.CommonLibrary.Controls.ValueEditors {

    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class IntEditor : NamedValueEditor<int> {

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue),
            typeof(int),
            typeof(IntEditor),
            new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue),
            typeof(int),
            typeof(IntEditor),
            new PropertyMetadata(int.MinValue));

        public IntEditor() : base() {
            this.InitializeComponent();
        }

        public int MaxValue {
            get { return (int)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        public int MinValue {
            get { return (int)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        public string ValueWrapper {
            get {
                return this.Value.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this.Value = MathHelper.Clamp(Convert.ToInt32(expression.Evaluate()), this.MinValue, this.MaxValue);
                }

                this.RaisePropertyChanged();
            }
        }

        protected override void OnValueChanged(int newValue, int oldValue, DependencyObject d) {
            if (d is IntEditor editor) {
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.ValueWrapper));
        }
    }
}
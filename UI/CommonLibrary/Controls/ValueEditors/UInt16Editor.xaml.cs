namespace Macabre2D.UI.CommonLibrary.Controls.ValueEditors {

    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class UInt16Editor : NamedValueEditor<ushort> {

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue),
            typeof(ushort),
            typeof(UInt16Editor),
            new PropertyMetadata(ushort.MaxValue));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue),
            typeof(ushort),
            typeof(UInt16Editor),
            new PropertyMetadata(ushort.MinValue));

        public UInt16Editor() : base() {
            this.InitializeComponent();
        }

        public ushort MaxValue {
            get { return (ushort)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        public ushort MinValue {
            get { return (ushort)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        public string ValueWrapper {
            get {
                return this.Value.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this.Value = (ushort)MathHelper.Clamp(Convert.ToUInt16(expression.Evaluate()), this.MinValue, this.MaxValue);
                }

                this.RaisePropertyChanged();
            }
        }

        protected override void OnValueChanged(ushort newValue, ushort oldValue, DependencyObject d) {
            if (d is UInt16Editor editor) {
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.ValueWrapper));
        }
    }
}
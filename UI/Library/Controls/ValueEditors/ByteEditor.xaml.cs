namespace Macabre2D.UI.Library.Controls.ValueEditors {

    using Microsoft.Xna.Framework;
    using System;
    using System.Windows;

    public partial class ByteEditor : NamedValueEditor<byte> {

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue),
            typeof(byte),
            typeof(ByteEditor),
            new PropertyMetadata(byte.MaxValue));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue),
            typeof(byte),
            typeof(ByteEditor),
            new PropertyMetadata(byte.MinValue));

        public ByteEditor() : base() {
            this.InitializeComponent();
        }

        public byte MaxValue {
            get { return (byte)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        public byte MinValue {
            get { return (byte)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        public string ValueWrapper {
            get {
                return this.Value.ToString();
            }

            set {
                var expression = new NCalc.Expression(value);

                if (!expression.HasErrors()) {
                    this.Value = (byte)MathHelper.Clamp(Convert.ToUInt32(expression.Evaluate()), this.MinValue, this.MaxValue);
                }

                this.RaisePropertyChanged();
            }
        }

        protected override void OnValueChanged(byte newValue, byte oldValue, DependencyObject d) {
            if (d is ByteEditor editor) {
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.ValueWrapper));
        }
    }
}
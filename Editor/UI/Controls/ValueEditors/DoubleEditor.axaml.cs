namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class DoubleEditor : BaseNumericEditor<double> {
        public DoubleEditor() {
            this.InitializeComponent();
        }

        protected override double ConvertValue(object calculatedValue) {
            return Convert.ToDouble(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
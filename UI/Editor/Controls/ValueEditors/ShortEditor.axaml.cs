namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using Avalonia.Markup.Xaml;

    public class ShortEditor : BaseNumericEditor<short> {
        public ShortEditor() {
            this.InitializeComponent();
        }

        protected override short ConvertValue(object calculatedValue) {
            return Convert.ToInt16(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class IntEditor : BaseNumericEditor<int> {
        public IntEditor() {
            this.InitializeComponent();
        }

        protected override int ConvertValue(object calculatedValue) {
            return Convert.ToInt32(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
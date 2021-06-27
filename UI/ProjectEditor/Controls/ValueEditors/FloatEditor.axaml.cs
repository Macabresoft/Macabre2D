namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class FloatEditor : BaseNumericEditor<float> {
        public FloatEditor() {
            this.InitializeComponent();
        }

        protected override float ConvertValue(object calculatedValue) {
            return Convert.ToSingle(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
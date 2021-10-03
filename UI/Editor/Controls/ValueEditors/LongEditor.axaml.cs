namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class LongEditor : BaseNumericEditor<long> {
        public LongEditor() {
            this.InitializeComponent();
        }

        protected override long ConvertValue(object calculatedValue) {
            return Convert.ToInt64(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
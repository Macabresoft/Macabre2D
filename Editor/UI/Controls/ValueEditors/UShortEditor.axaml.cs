namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class UShortEditor : BaseNumericEditor<ushort> {
        public UShortEditor() {
            this.InitializeComponent();
        }

        protected override ushort ConvertValue(object calculatedValue) {
            return Convert.ToUInt16(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
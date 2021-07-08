namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors {
    using System;
    using Avalonia.Markup.Xaml;

    public class ULongEditor : BaseNumericEditor<ulong> {
        public ULongEditor() {
            this.InitializeComponent();
        }

        protected override ulong ConvertValue(object calculatedValue) {
            return Convert.ToUInt64(calculatedValue);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
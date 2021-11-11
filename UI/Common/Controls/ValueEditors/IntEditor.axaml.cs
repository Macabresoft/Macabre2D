namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using Avalonia.Markup.Xaml;
    using Unity;

    public class IntEditor : BaseNumericEditor<int> {
        public IntEditor() : this(null) {
        }

        [InjectionConstructor]
        public IntEditor(ValueControlDependencies dependencies) : base(dependencies) {
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
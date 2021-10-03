namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;

    public class StringEditor : ValueEditorControl<string> {
        public static readonly DirectProperty<StringEditor, string> IntermediaryValueProperty =
            AvaloniaProperty.RegisterDirect<StringEditor, string>(
                nameof(IntermediaryValue),
                editor => editor.IntermediaryValue,
                (editor, value) => editor.IntermediaryValue = value);

        private string _intermediaryValue;

        public StringEditor() {
            this.InitializeComponent();
        }

        public string IntermediaryValue {
            get => this._intermediaryValue;
            set {
                if (!this.UpdateOnLostFocus) {
                    this.SetEditorValue(this.Value, value);
                }

                this.SetAndRaise(IntermediaryValueProperty, ref this._intermediaryValue, value);
            }
        }

        protected override void OnValueChanged() {
            base.OnValueChanged();

            if (this.HasValueChanged()) {
                this.SetAndRaise(IntermediaryValueProperty, ref this._intermediaryValue, this.Value);
            }
        }

        private bool HasValueChanged() {
            if (this.Value != null) {
                return !this.Value.Equals(this.IntermediaryValue);
            }

            return this.IntermediaryValue != null;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ValueEditor_OnLostFocus(object sender, RoutedEventArgs e) {
            if (this.UpdateOnLostFocus && this.HasValueChanged()) {
                this.SetEditorValue(this.Value, this.IntermediaryValue);
            }
        }
    }
}
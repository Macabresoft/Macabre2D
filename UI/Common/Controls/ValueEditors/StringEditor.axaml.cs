namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Unity;

public class StringEditor : ValueEditorControl<string> {
    public static readonly DirectProperty<StringEditor, string> IntermediaryValueProperty =
        AvaloniaProperty.RegisterDirect<StringEditor, string>(
            nameof(IntermediaryValue),
            editor => editor.IntermediaryValue,
            (editor, value) => editor.IntermediaryValue = value);

    public static readonly StyledProperty<string> WatermarkProperty =
        AvaloniaProperty.Register<StringEditor, string>(nameof(Watermark));

    private string _intermediaryValue;

    public StringEditor() : this(null) {
    }

    [InjectionConstructor]
    public StringEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();

        if (dependencies != null && string.IsNullOrEmpty(this.Watermark)) {
            this.Watermark = dependencies.Title;
        }
    }

    public string IntermediaryValue {
        get => this._intermediaryValue;
        set {
            if (!this.UpdateOnLostFocus) {
                this.Value = value;
            }

            this.SetAndRaise(IntermediaryValueProperty, ref this._intermediaryValue, value);
        }
    }

    public string Watermark {
        get => this.GetValue(WatermarkProperty);
        set => this.SetValue(WatermarkProperty, value);
    }

    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (this.HasValueChanged()) {
            this.IntermediaryValue = this.Value;
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
            this.Value = this.IntermediaryValue;
        }
    }
}
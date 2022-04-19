namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class RotationEditor : ValueEditorControl<Rotation> {
    public static readonly DirectProperty<RotationEditor, float> DisplayValueProperty =
        AvaloniaProperty.RegisterDirect<RotationEditor, float>(
            nameof(DisplayValue),
            editor => editor.DisplayValue,
            (editor, value) => editor.DisplayValue = value);

    private float _displayValue;

    public RotationEditor() : this(null) {
    }

    [InjectionConstructor]
    public RotationEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    public float DisplayValue {
        get => this._displayValue;
        set => this.SetAndRaise(DisplayValueProperty, ref this._displayValue, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValue();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        switch (change.Property.Name) {
            case nameof(this.Value):
                this.UpdateDisplayValue();
                break;
            case nameof(this.DisplayValue):
                Dispatcher.UIThread.Post(() =>
                    this.SetValue(ValueProperty, Rotation.CreateFromDegrees(this.DisplayValue)));
                break;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplayValue() {
        this._displayValue = this.Value.Degrees;

        Dispatcher.UIThread.Post(() => { this.RaisePropertyChanged(DisplayValueProperty, Optional<float>.Empty, this.DisplayValue); });
    }
}
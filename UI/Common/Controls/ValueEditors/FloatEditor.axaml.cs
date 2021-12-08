namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public class FloatEditor : BaseNumericEditor<float> {
    public FloatEditor() : this(null) {
    }

    [InjectionConstructor]
    public FloatEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override float ConvertValue(object calculatedValue) {
        return Convert.ToSingle(calculatedValue);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
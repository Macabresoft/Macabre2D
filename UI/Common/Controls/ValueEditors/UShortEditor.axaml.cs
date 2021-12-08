namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public class UShortEditor : BaseNumericEditor<ushort> {
    public UShortEditor() : this(null) {
    }

    [InjectionConstructor]
    public UShortEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override ushort ConvertValue(object calculatedValue) {
        return Convert.ToUInt16(calculatedValue);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
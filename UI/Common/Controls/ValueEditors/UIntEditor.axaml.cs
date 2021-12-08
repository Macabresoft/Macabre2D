namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public class UIntEditor : BaseNumericEditor<uint> {
    public UIntEditor() : this(null) {
    }

    [InjectionConstructor]
    public UIntEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override uint ConvertValue(object calculatedValue) {
        return Convert.ToUInt32(calculatedValue);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
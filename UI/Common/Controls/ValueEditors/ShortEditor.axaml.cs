namespace Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public partial class ShortEditor : BaseNumericEditor<short> {
    public ShortEditor() : this(null) {
    }

    [InjectionConstructor]
    public ShortEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override short ConvertValue(object calculatedValue) {
        return Convert.ToInt16(calculatedValue);
    }
}
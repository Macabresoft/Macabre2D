namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public partial class DoubleEditor : BaseNumericEditor<double> {
    public DoubleEditor() : this(null) {
    }

    [InjectionConstructor]
    public DoubleEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override double ConvertValue(object calculatedValue) {
        return Convert.ToDouble(calculatedValue);
    }
}
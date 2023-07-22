namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public partial class ULongEditor : BaseNumericEditor<ulong> {
    public ULongEditor() : this(null) {
    }

    [InjectionConstructor]
    public ULongEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override ulong ConvertValue(object calculatedValue) {
        return Convert.ToUInt64(calculatedValue);
    }
}
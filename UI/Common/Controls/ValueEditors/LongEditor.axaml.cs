namespace Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public partial class LongEditor : BaseNumericEditor<long> {
    public LongEditor() : this(null) {
    }

    [InjectionConstructor]
    public LongEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override long ConvertValue(object calculatedValue) {
        return Convert.ToInt64(calculatedValue);
    }
}
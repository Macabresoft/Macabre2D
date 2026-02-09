namespace Macabre2D.UI.Common;

using System;
using Avalonia.Markup.Xaml;
using Unity;

public partial class ByteEditor : BaseNumericEditor<byte> {
    public ByteEditor() : this(null) {
    }

    [InjectionConstructor]
    public ByteEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    protected override byte ConvertValue(object calculatedValue) {
        return Convert.ToByte(calculatedValue);
    }
}
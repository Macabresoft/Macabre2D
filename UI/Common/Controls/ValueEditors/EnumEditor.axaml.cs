namespace Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Unity;

public partial class EnumEditor : ValueEditorControl<object> {
    public static readonly StyledProperty<Type> EnumTypeProperty =
        AvaloniaProperty.Register<EnumEditor, Type>(nameof(EnumType));

    public EnumEditor() : this(null) {
    }

    [InjectionConstructor]
    public EnumEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.EnumType = dependencies?.ValueType;
        this.InitializeComponent();
    }
    
    public Type EnumType {
        get => this.GetValue(EnumTypeProperty);
        set => this.SetValue(EnumTypeProperty, value);
    }
}
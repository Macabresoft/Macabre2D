namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Unity;

public class EnumEditor : ValueEditorControl<object> {
    public static readonly DirectProperty<EnumEditor, Type> EnumTypeProperty =
        AvaloniaProperty.RegisterDirect<EnumEditor, Type>(
            nameof(EnumType),
            editor => editor.EnumType);

    public EnumEditor() : this(null) {
    }

    [InjectionConstructor]
    public EnumEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.EnumType = dependencies?.ValueType;
        this.InitializeComponent();
    }

    public Type EnumType { get; }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
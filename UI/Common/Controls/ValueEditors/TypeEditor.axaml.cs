namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Unity;

public partial class TypeEditor : ValueEditorControl<Type> {
    public static readonly DirectProperty<TypeEditor, IReadOnlyCollection<Type>> AvailableTypesProperty =
        AvaloniaProperty.RegisterDirect<TypeEditor, IReadOnlyCollection<Type>>(
            nameof(AvailableTypes),
            editor => editor.AvailableTypes);

    public TypeEditor() : this(null, Resolver.Resolve<IAssemblyService>()) {
    }

    [InjectionConstructor]
    public TypeEditor(ValueControlDependencies dependencies, IAssemblyService assemblyService) : base(dependencies) {
        if (dependencies != null) {
            this.AvailableTypes = assemblyService.LoadTypes(dependencies.ValueType).ToList();
        }
        
        this.InitializeComponent();
    }
    
    public IReadOnlyCollection<Type> AvailableTypes { get; }
}
namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class NamedSettingCollectionEditor : ValueEditorControl<NamedSettingCollection> {
    public NamedSettingCollectionEditor() : this(null) {
    }

    [InjectionConstructor]
    public NamedSettingCollectionEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    public IReadOnlyCollection<Type> AvailableType { get; } = new[] {
        typeof(bool),
        typeof(string),
        typeof(float),
        typeof(int)
    };
}
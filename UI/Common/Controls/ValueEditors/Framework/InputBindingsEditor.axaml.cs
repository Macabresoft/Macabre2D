namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class InputBindingsEditor : ValueEditorControl<InputBindings> {
    public InputBindingsEditor() : this(null) {
    }

    [InjectionConstructor]
    public InputBindingsEditor(ValueControlDependencies dependencies) : base(dependencies) {
        var actions = Enum.GetValues<InputAction>().ToList();
        actions.Remove(InputAction.None);
        this.Actions = actions;

        this.InitializeComponent();
    }

    public IReadOnlyCollection<InputAction> Actions { get; }
}
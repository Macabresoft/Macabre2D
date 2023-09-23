namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class InputActionEditor : ValueEditorControl<InputAction> {
    public InputActionEditor() : this(null) {
    }

    [InjectionConstructor]
    public InputActionEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.EnabledInputActions = this.GetEnabledInputActions();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<InputAction> EnabledInputActions { get; }

    private IReadOnlyCollection<InputAction> GetEnabledInputActions() {
        var actions = Enum.GetValues<InputAction>().ToList();
        actions.Insert(0, InputAction.None);
        return actions;
    }
}
namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class InputActionEditor : ValueEditorControl<InputAction> {
    private readonly IProjectService _projectService;

    public InputActionEditor() : this(null, Resolver.Resolve<IProjectService>()) {
    }

    [InjectionConstructor]
    public InputActionEditor(ValueControlDependencies dependencies, IProjectService projectService) : base(dependencies) {
        this._projectService = projectService;
        this.EnabledInputActions = this.GetEnabledInputActions();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<InputAction> EnabledInputActions { get; }

    private IReadOnlyCollection<InputAction> GetEnabledInputActions() {
        var actions = Enum.GetValues<InputAction>()
            .Where(x => this._projectService.CurrentProject.Settings.InputSettings.IsActionEnabled(x))
            .ToList();

        actions.Insert(0, InputAction.None);
        return actions;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
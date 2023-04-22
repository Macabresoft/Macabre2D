namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class InputSettingsEditor : ValueEditorControl<InputSettings> {
    public InputSettingsEditor() : this(null) {
    }

    [InjectionConstructor]
    public InputSettingsEditor(ValueControlDependencies dependencies) : base(dependencies) {
        var actions = Enum.GetValues<InputAction>().ToList();
        actions.Remove(InputAction.None);
        this.Actions = actions;

        this.InitializeComponent();
    }

    public IReadOnlyCollection<InputAction> Actions { get; }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
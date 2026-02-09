namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Macabre2D.Project.Common;
using ReactiveUI;
using Unity;

public partial class LayersEditor : ValueEditorControl<Layers> {
    public static readonly DirectProperty<FlagsEnumEditor, ICommand> SelectAllCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(SelectAllCommand),
            editor => editor.SelectAllCommand);

    private readonly IProjectService _projectService;

    public LayersEditor() : this(null, Resolver.Resolve<IProjectService>()) {
    }

    [InjectionConstructor]
    public LayersEditor(ValueControlDependencies dependencies, IProjectService projectService) : base(dependencies) {
        this._projectService = projectService;
        var enabledLayers = Enum.GetValues<Layers>().ToList();
        enabledLayers.Remove(Layers.None);
        this.EnabledLayers = enabledLayers;
        this.ClearCommand = ReactiveCommand.Create(this.Clear);
        this.SelectAllCommand = ReactiveCommand.Create(this.SelectAll);
        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public IReadOnlyCollection<Layers> EnabledLayers { get; }

    public ICommand SelectAllCommand { get; }

    private void Clear() {
        this.Value = Layers.None;
    }

    private void SelectAll() {
        var all = this._projectService.CurrentProject.AllLayers;

        if (all != this.Value) {
            this.Value = all;
        }
    }
}
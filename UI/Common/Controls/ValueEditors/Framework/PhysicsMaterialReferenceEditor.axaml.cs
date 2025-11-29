namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class PhysicsMaterialReferenceEditor : ValueEditorControl<PhysicsMaterialReference> {

    public static readonly DirectProperty<PhysicsMaterialReferenceEditor, IReadOnlyCollection<PhysicsMaterial>> AvailableMaterialsProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialReferenceEditor, IReadOnlyCollection<PhysicsMaterial>>(
            nameof(AvailableMaterials),
            editor => editor.AvailableMaterials);

    public static readonly DirectProperty<PhysicsMaterialReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<PhysicsMaterialReferenceEditor, PhysicsMaterial> SelectedMaterialProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialReferenceEditor, PhysicsMaterial>(
            nameof(SelectedMaterial),
            editor => editor.SelectedMaterial,
            (editor, value) => editor.SelectedMaterial = value);

    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;

    public PhysicsMaterialReferenceEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public PhysicsMaterialReferenceEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;
        this.ClearCommand = ReactiveCommand.Create(this.Clear);

        this.InitializeComponent();
    }

    public IReadOnlyCollection<PhysicsMaterial> AvailableMaterials => this._projectService.CurrentProject.PhysicsMaterials;

    public ICommand ClearCommand { get; }

    public PhysicsMaterial SelectedMaterial {
        get => this.Value.Material;
        set {
            var originalValue = this.SelectedMaterial;
            this._undoService.Do(() =>
            {
                this.Value.Id = value.Id;
                this.RaisePropertyChanged(SelectedMaterialProperty, originalValue, this.SelectedMaterial);
            }, () =>
            {
                this.Value.Id = originalValue.Id;
                this.RaisePropertyChanged(SelectedMaterialProperty, this.SelectedMaterial, originalValue);
            });
        }
    }

    private void Clear() {
        if (this.Value.Id != Guid.Empty) {
            this.SelectedMaterial = PhysicsMaterial.Default;
        }
    }
}
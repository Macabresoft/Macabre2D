namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class PhysicsMaterialGuidEditor : ValueEditorControl<Guid> {

    public static readonly DirectProperty<PhysicsMaterialGuidEditor, IReadOnlyCollection<PhysicsMaterial>> AvailableMaterialsProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialGuidEditor, IReadOnlyCollection<PhysicsMaterial>>(
            nameof(AvailableMaterials),
            editor => editor.AvailableMaterials);

    public static readonly DirectProperty<PhysicsMaterialGuidEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialGuidEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<PhysicsMaterialGuidEditor, PhysicsMaterial> SelectedMaterialProperty =
        AvaloniaProperty.RegisterDirect<PhysicsMaterialGuidEditor, PhysicsMaterial>(
            nameof(SelectedMaterial),
            editor => editor.SelectedMaterial,
            (editor, value) => editor.SelectedMaterial = value);

    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;
    private PhysicsMaterial _selectedMaterial = PhysicsMaterial.Default;
    private bool _shouldUpdate;

    public PhysicsMaterialGuidEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public PhysicsMaterialGuidEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;
        this.ClearCommand = ReactiveCommand.Create(this.Clear);

        this.InitializeComponent();
        this._shouldUpdate = true;
        this.ResetSelectedMaterial();
    }

    public IReadOnlyCollection<PhysicsMaterial> AvailableMaterials => this._projectService.CurrentProject.PhysicsMaterials;

    public ICommand ClearCommand { get; }

    public PhysicsMaterial SelectedMaterial {
        get => this._selectedMaterial;
        set {
            var originalValue = this.SelectedMaterial;
            this._undoService.Do(() => { this.Value = value.Id; }, () => { this.Value = originalValue.Id; });
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<Guid> args) {
        base.OnValueChanged(args);
        this.ResetSelectedMaterial();
    }

    private void Clear() {
        if (this.Value != Guid.Empty) {
            var originalValue = this.Value;
            var originalMaterial = this._selectedMaterial;
            this._undoService.Do(() =>
            {
                try {
                    this._shouldUpdate = false;
                    this.Value = Guid.Empty;
                    this._selectedMaterial = PhysicsMaterial.Default;
                }
                finally {
                    this._shouldUpdate = true;
                }
            }, () =>
            {
                try {
                    this._shouldUpdate = false;
                    this.Value = originalValue;
                    this._selectedMaterial = originalMaterial;
                }
                finally {
                    this._shouldUpdate = true;
                }
            });
        }
    }

    private void ResetSelectedMaterial() {
        if (this._shouldUpdate) {
            try {
                this._shouldUpdate = false;
                var originalValue = this._selectedMaterial;
                this._selectedMaterial = this._projectService.CurrentProject.PhysicsMaterials.Get(this.Value);
                this.RaisePropertyChanged(SelectedMaterialProperty, this.SelectedMaterial, originalValue);
            }
            finally {
                this._shouldUpdate = true;
            }
        }
    }
}
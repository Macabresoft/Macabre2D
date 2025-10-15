namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class RenderStepReferenceEditor : ValueEditorControl<RenderStepReference> {

    public static readonly DirectProperty<RenderStepReferenceEditor, IReadOnlyCollection<RenderStep>> AvailableStepsProperty =
        AvaloniaProperty.RegisterDirect<RenderStepReferenceEditor, IReadOnlyCollection<RenderStep>>(
            nameof(AvailableSteps),
            editor => editor.AvailableSteps);

    public static readonly DirectProperty<RenderStepReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<RenderStepReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<RenderStepReferenceEditor, RenderStep> SelectedStepProperty =
        AvaloniaProperty.RegisterDirect<RenderStepReferenceEditor, RenderStep>(
            nameof(SelectedStep),
            editor => editor.SelectedStep,
            (editor, value) => editor.SelectedStep = value);

    private readonly ObservableCollectionExtended<RenderStep> _availableSteps = [];

    private readonly IUndoService _undoService;
    private bool _canSetStep = true;
    private RenderStep _selectedStep;

    public RenderStepReferenceEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RenderStepReferenceEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;

        var steps = projectService.CurrentProject.RenderSteps.OfType<RenderStep>().ToList();
        steps.Insert(0, null);
        this._availableSteps.Reset(steps);
        this.ResetSelectedStep();

        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.WhenAny(x => x.SelectedStep, y => y.Value != null));

        this.InitializeComponent();
    }

    public IReadOnlyCollection<RenderStep> AvailableSteps => this._availableSteps;

    public ICommand ClearCommand { get; }

    public RenderStep SelectedStep {
        get => this._selectedStep;
        set {
            this.SetAndRaise(SelectedStepProperty, ref this._selectedStep, value);
            this.SetStep(this._selectedStep);
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<RenderStepReference> args) {
        base.OnValueChanged(args);

        if (this.Value != null && this.Value.Id != Guid.Empty) {
            this.ResetSelectedStep();
        }
    }

    private void Clear() {
        if (this.Value.Id != Guid.Empty) {
            this.SelectedStep = null;
        }
        else {
            try {
                this._canSetStep = false;
                this.Value.Step = null;
                this.SelectedStep = null;
            }
            finally {
                this._canSetStep = true;
            }
        }
    }

    private void ResetSelectedStep() {
        try {
            this._canSetStep = false;
            this.SelectedStep = this.AvailableSteps.FirstOrDefault(x => x != null && x.Id == this.Value.Id);
        }
        finally {
            this._canSetStep = true;
        }
    }

    private void SetStep(RenderStep step) {
        if (this._canSetStep) {
            var originalValue = this.Value.Step;
            this._undoService.Do(
                () =>
                {
                    try {
                        this._canSetStep = false;
                        this.Value.Step = step;
                        this.SelectedStep = step;
                    }
                    finally {
                        this._canSetStep = true;
                    }
                },
                () =>
                {
                    try {
                        this._canSetStep = false;
                        this.Value.Step = originalValue;
                        this.SelectedStep = originalValue;
                    }
                    finally {
                        this._canSetStep = true;
                    }
                });
        }
    }
}
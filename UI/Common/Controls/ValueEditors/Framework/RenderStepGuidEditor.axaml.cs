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

public partial class RenderStepGuidEditor : ValueEditorControl<Guid> {

    public static readonly DirectProperty<RenderStepGuidEditor, IReadOnlyCollection<RenderStep>> AvailableStepsProperty =
        AvaloniaProperty.RegisterDirect<RenderStepGuidEditor, IReadOnlyCollection<RenderStep>>(
            nameof(AvailableSteps),
            editor => editor.AvailableSteps);

    public static readonly DirectProperty<RenderStepGuidEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<RenderStepGuidEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<RenderStepGuidEditor, RenderStep> SelectedStepProperty =
        AvaloniaProperty.RegisterDirect<RenderStepGuidEditor, RenderStep>(
            nameof(SelectedStep),
            editor => editor.SelectedStep,
            (editor, value) => editor.SelectedStep = value);

    private readonly ObservableCollectionExtended<RenderStep> _availableSteps = [];
    private readonly IProjectService _projectService;
    private readonly Type _renderStepType = typeof(RenderStep);
    private readonly IUndoService _undoService;
    private bool _canSetStep = true;

    public RenderStepGuidEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RenderStepGuidEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;

        if (dependencies is { Owner: IRenderStepReference reference, ValuePropertyName: nameof(IRenderStepReference.Id) }) {
            this._renderStepType = reference.Type;
        }

        var steps = this._projectService.CurrentProject.RenderSteps
            .OfType<RenderStep>()
            .Where(x => x.GetType().IsAssignableTo(this._renderStepType))
            .ToList();

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
        get;
        set {
            this.SetAndRaise(SelectedStepProperty, ref field, value);
            this.SetStep(field);
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<Guid> args) {
        base.OnValueChanged(args);

        if (this.Value != Guid.Empty) {
            this.ResetSelectedStep();
        }
    }

    private void Clear() {
        if (this.Value != Guid.Empty) {
            this.SelectedStep = null;
        }
        else {
            try {
                this._canSetStep = false;
                this.Value = Guid.Empty;
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
            this.SelectedStep = this.AvailableSteps.FirstOrDefault(x => x != null && x.Id == this.Value);
        }
        finally {
            this._canSetStep = true;
        }
    }

    private void SetStep(RenderStep step) {
        if (this._canSetStep) {
            var originalValue = this.Value;
            var originalStep = this._projectService.CurrentProject.RenderSteps.OfType<RenderStep>().FirstOrDefault(x => x.Id == originalValue);
            var updatedValue = step?.Id ?? Guid.Empty;
            this._undoService.Do(
                () =>
                {
                    try {
                        this._canSetStep = false;
                        this.Value = updatedValue;
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
                        this.Value = originalValue;
                        this.SelectedStep = originalStep;
                    }
                    finally {
                        this._canSetStep = true;
                    }
                });
        }
    }
}
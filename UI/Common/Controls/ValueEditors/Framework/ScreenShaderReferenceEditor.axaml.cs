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

public partial class ScreenShaderReferenceEditor : ValueEditorControl<ScreenShaderReference> {

    public static readonly DirectProperty<ScreenShaderReferenceEditor, IReadOnlyCollection<ScreenShader>> AvailableShadersProperty =
        AvaloniaProperty.RegisterDirect<ScreenShaderReferenceEditor, IReadOnlyCollection<ScreenShader>>(
            nameof(AvailableShaders),
            editor => editor.AvailableShaders);

    public static readonly DirectProperty<ScreenShaderReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<ScreenShaderReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<ScreenShaderReferenceEditor, ScreenShader> SelectedShaderProperty =
        AvaloniaProperty.RegisterDirect<ScreenShaderReferenceEditor, ScreenShader>(
            nameof(SelectedShader),
            editor => editor.SelectedShader,
            (editor, value) => editor.SelectedShader = value);

    private readonly ObservableCollectionExtended<ScreenShader> _availableShaders = new();
    private readonly IProjectService _projectService;

    private readonly IUndoService _undoService;
    private bool _canSetShader = true;
    private ScreenShader _selectedShader;

    public ScreenShaderReferenceEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ScreenShaderReferenceEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;

        var screenShaders = this._projectService.CurrentProject.RenderSteps.OfType<ScreenShader>().ToList();
        screenShaders.Insert(0, null);
        this._availableShaders.Reset(screenShaders);
        this.ResetSelectedShader();

        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.WhenAny(x => x.SelectedShader, y => y.Value != null));

        this.InitializeComponent();
    }

    public IReadOnlyCollection<ScreenShader> AvailableShaders => this._availableShaders;

    public ICommand ClearCommand { get; }

    public ScreenShader SelectedShader {
        get => this._selectedShader;
        set {
            this.SetAndRaise(SelectedShaderProperty, ref this._selectedShader, value);
            this.SetShader(this._selectedShader);
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<ScreenShaderReference> args) {
        base.OnValueChanged(args);

        if (this.Value != null && this.Value.Id != Guid.Empty) {
            this.ResetSelectedShader();
        }
    }

    private void Clear() {
        if (this.Value.Id != Guid.Empty) {
            this.SelectedShader = null;
        }
        else {
            try {
                this._canSetShader = false;
                this.Value.Shader = null;
                this.SelectedShader = null;
            }
            finally {
                this._canSetShader = true;
            }
        }
    }

    private void ResetSelectedShader() {
        try {
            this._canSetShader = false;
            this.SelectedShader = this.AvailableShaders.FirstOrDefault(x => x != null && x.Id == this.Value.Id);
        }
        finally {
            this._canSetShader = true;
        }
    }

    private void SetShader(ScreenShader shader) {
        if (this._canSetShader) {
            var originalValue = this.Value.Shader;
            this._undoService.Do(
                () =>
                {
                    try {
                        this._canSetShader = false;
                        this.Value.Shader = shader;
                        this.SelectedShader = shader;
                    }
                    finally {
                        this._canSetShader = true;
                    }
                },
                () =>
                {
                    try {
                        this._canSetShader = false;
                        this.Value.Shader = originalValue;
                        this.SelectedShader = originalValue;
                    }
                    finally {
                        this._canSetShader = true;
                    }
                });
        }
    }
}
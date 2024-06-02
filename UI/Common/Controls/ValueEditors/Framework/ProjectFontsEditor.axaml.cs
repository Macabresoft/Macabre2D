namespace Macabresoft.Macabre2D.UI.Common;

using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class ProjectFontsEditor : ValueEditorControl<ProjectFonts> {
    public static readonly DirectProperty<ProjectFontsEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    private readonly IProjectService _projectService;

    private readonly IUndoService _undoService;

    public ProjectFontsEditor() : this(
        null,
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ProjectFontsEditor(
        ValueControlDependencies dependencies,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._projectService = projectService;
        this._undoService = undoService;

        this.ClearCommand = ReactiveCommand.Create(this.Clear);

        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    private void Clear() {
        // TODO: clear current
    }
}
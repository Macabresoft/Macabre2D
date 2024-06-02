namespace Macabresoft.Macabre2D.UI.Common;

using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using ReactiveUI;
using Unity;

public partial class ProjectFontsEditor : ValueEditorControl<ProjectFonts> {
    public static readonly DirectProperty<ProjectFontsEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<ProjectFontsEditor, ResourceCulture> CurrentCultureProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ResourceCulture>(
            nameof(CurrentCulture),
            editor => editor.CurrentCulture,
            (editor, value) => editor.CurrentCulture = value);

    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;
    private ResourceCulture _currentCulture = ResourceCulture.Default;

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

        this.ClearCommand = ReactiveCommand.Create<FontCategory>(this.Clear);

        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public ResourceCulture CurrentCulture {
        get => this._currentCulture;
        set {
            if (value != this._currentCulture) {
                this.SetAndRaise(CurrentCultureProperty, ref this._currentCulture, value);
                // TODO: reset list of categories with stored values from the project
            }
        }
    }

    private void Clear(FontCategory category) {
        if (category != FontCategory.None) {
            var key = new ProjectFontKey(category, this.CurrentCulture);
            if (this._projectService.CurrentProject.Fonts.TryGetFont(key, out var fontDefinition)) {
                this._undoService.Do(() => { this._projectService.CurrentProject.Fonts.RemoveFont(category, this.CurrentCulture); }, () => { this._projectService.CurrentProject.Fonts.SetFont(key, fontDefinition); });
            }
        }
    }
}
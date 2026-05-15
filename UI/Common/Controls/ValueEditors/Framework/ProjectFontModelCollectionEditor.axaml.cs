namespace Macabre2D.UI.Common;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabre2D.Framework;
using Macabresoft.AvaloniaEx;
using ReactiveUI;
using Unity;

public partial class ProjectFontModelCollectionEditor : ValueEditorControl<ProjectFontModelCollection> {


    public static readonly DirectProperty<ProjectFontModelCollectionEditor, ICommand> ClearMonoGameFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontModelCollectionEditor, ICommand>(
            nameof(ClearMonoGameFontCommand),
            editor => editor.ClearMonoGameFontCommand);

    public static readonly DirectProperty<ProjectFontModelCollectionEditor, ICommand> ClearSpriteSheetFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontModelCollectionEditor, ICommand>(
            nameof(ClearSpriteSheetFontCommand),
            editor => editor.ClearSpriteSheetFontCommand);

    public static readonly DirectProperty<ProjectFontModelCollectionEditor, ICommand> SelectMonoGameFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontModelCollectionEditor, ICommand>(
            nameof(SelectMonoGameFontCommand),
            editor => editor.SelectMonoGameFontCommand);

    public static readonly DirectProperty<ProjectFontModelCollectionEditor, ICommand> SelectSpriteSheetFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontModelCollectionEditor, ICommand>(
            nameof(SelectSpriteSheetFontCommand),
            editor => editor.SelectSpriteSheetFontCommand);

    public static readonly DirectProperty<ProjectFontModelCollectionEditor, bool> ShouldRenderInScreenSpaceProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontModelCollectionEditor, bool>(
            nameof(ShouldRenderInScreenSpace),
            editor => editor.ShouldRenderInScreenSpace,
            (editor, value) => editor.ShouldRenderInScreenSpace = value);

    private readonly ICommonDialogService _dialogService;
    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;

    public ProjectFontModelCollectionEditor() : this(
        null,
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IProjectService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ProjectFontModelCollectionEditor(
        ValueControlDependencies dependencies,
        ICommonDialogService dialogService,
        IProjectService projectService,
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
        this._projectService = projectService;
        this._undoService = undoService;

        this.ClearMonoGameFontCommand = ReactiveCommand.Create<ProjectFontModel>(this.ClearMonoGameFont);
        this.ClearSpriteSheetFontCommand = ReactiveCommand.Create<ProjectFontModel>(this.ClearSpriteSheetFont);
        this.SelectMonoGameFontCommand = ReactiveCommand.CreateFromTask<ProjectFontModel>(this.SelectMonoGameFont);
        this.SelectSpriteSheetFontCommand = ReactiveCommand.CreateFromTask<ProjectFontModel>(this.SelectSpriteSheetFont);
        this.InitializeComponent();
    }

    public ICommand ClearMonoGameFontCommand { get; }

    public ICommand ClearSpriteSheetFontCommand { get; }

    public ICommand SelectMonoGameFontCommand { get; }

    public ICommand SelectSpriteSheetFontCommand { get; }

    public bool ShouldRenderInScreenSpace {
        get => this._projectService.CurrentProject.Fonts.CheckShouldRenderInScreenSpace(this.Value.Culture);
        set {
            var originalValue = this.ShouldRenderInScreenSpace;
            if (value != originalValue) {
                this._undoService.Do(() =>
                {
                    this._projectService.CurrentProject.Fonts.SetShouldRenderInScreenSpace(this.Value.Culture, value);
                    this.RaisePropertyChanged(ShouldRenderInScreenSpaceProperty, originalValue, value);
                }, () =>
                {
                    this._projectService.CurrentProject.Fonts.SetShouldRenderInScreenSpace(this.Value.Culture, originalValue);
                    this.RaisePropertyChanged(ShouldRenderInScreenSpaceProperty, value, originalValue);
                });
            }
        }
    }

    /// <inheritdoc />
    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<ProjectFontModelCollection> args) {
        base.OnValueChanged(args);

        if (args.NewValue.Value is { } collection) {
            foreach (var model in collection.Models) {
                model.ResetDisplayProperties();
            }
        }
    }

    private void ClearMonoGameFont(ProjectFontModel font) {
        font.Definition = font.Definition.WithLegacyFont(Guid.Empty);
    }

    private void ClearSpriteSheetFont(ProjectFontModel font) {
        font.Definition = font.Definition.WithSpriteSheetFont(Guid.Empty, Guid.Empty);
    }

    private async Task SelectMonoGameFont(ProjectFontModel font) {
        var contentNode = await this._dialogService.OpenContentSelectionDialog(typeof(LegacyFontAsset), false, this.Title);
        if (contentNode != null && contentNode.Id != Guid.Empty) {
            var contentId = contentNode.Id;
            var originalDefinition = font.Definition;
            var newDefinition = originalDefinition.WithLegacyFont(contentId);
            this._undoService.Do(
                () => { font.Definition = newDefinition; },
                () => { font.Definition = originalDefinition; });
        }
    }

    private async Task SelectSpriteSheetFont(ProjectFontModel font) {
        var (spriteSheet, packagedAssetId) = await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteSheetFont>(this.Title);
        if (spriteSheet != null && packagedAssetId != Guid.Empty) {
            var contentId = spriteSheet.ContentId;
            var originalDefinition = font.Definition;
            var newDefinition = originalDefinition.WithSpriteSheetFont(contentId, packagedAssetId);
            this._undoService.Do(
                () => { font.Definition = newDefinition; },
                () => { font.Definition = originalDefinition; });
        }
    }
}
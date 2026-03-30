namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Microsoft.Xna.Framework.Graphics;
using ReactiveUI;
using Unity;

public partial class ProjectFontsEditor : ValueEditorControl<ProjectFonts> {

    public static readonly DirectProperty<ProjectFontsEditor, IReadOnlyCollection<ResourceCulture>> AvailableCulturesProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, IReadOnlyCollection<ResourceCulture>>(
            nameof(AvailableCultures),
            editor => editor.AvailableCultures);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> ClearMonoGameFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(ClearMonoGameFontCommand),
            editor => editor.ClearMonoGameFontCommand);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> ClearSpriteSheetFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(ClearSpriteSheetFontCommand),
            editor => editor.ClearSpriteSheetFontCommand);

    public static readonly DirectProperty<ProjectFontsEditor, ResourceCulture> CurrentCultureProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ResourceCulture>(
            nameof(CurrentCulture),
            editor => editor.CurrentCulture,
            (editor, value) => editor.CurrentCulture = value);

    public static readonly DirectProperty<ProjectFontsEditor, IReadOnlyCollection<ProjectFontModel>> FontsInCurrentCultureProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, IReadOnlyCollection<ProjectFontModel>>(
            nameof(FontsInCurrentCulture),
            editor => editor.FontsInCurrentCulture);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> SelectMonoGameFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(SelectMonoGameFontCommand),
            editor => editor.SelectMonoGameFontCommand);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> SelectSpriteSheetFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(SelectSpriteSheetFontCommand),
            editor => editor.SelectSpriteSheetFontCommand);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly ObservableCollectionExtended<ProjectFontModel> _fontsInCurrentCulture = [];
    private readonly IUndoService _undoService;

    public ProjectFontsEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ProjectFontsEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;

        this.AvailableCultures = Enum.GetValues<ResourceCulture>().ToList();
        this.ClearMonoGameFontCommand = ReactiveCommand.Create<ProjectFontModel>(this.ClearMonoGameFont);
        this.ClearSpriteSheetFontCommand = ReactiveCommand.Create<ProjectFontModel>(this.ClearSpriteSheetFont);
        this.SelectMonoGameFontCommand = ReactiveCommand.CreateFromTask<ProjectFontModel>(this.SelectMonoGameFont);
        this.SelectSpriteSheetFontCommand = ReactiveCommand.CreateFromTask<ProjectFontModel>(this.SelectSpriteSheetFont);
        this.ResetFontCategories();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<ResourceCulture> AvailableCultures { get; }

    public ICommand ClearMonoGameFontCommand { get; }

    public ICommand ClearSpriteSheetFontCommand { get; }


    public ResourceCulture CurrentCulture {
        get;
        set {
            if (value != field) {
                this.SetAndRaise(CurrentCultureProperty, ref field, value);
                this.ResetFontCategories();
            }
        }
    } = ResourceCulture.Default;

    public IReadOnlyCollection<ProjectFontModel> FontsInCurrentCulture => this._fontsInCurrentCulture;

    public ICommand SelectMonoGameFontCommand { get; }

    public ICommand SelectSpriteSheetFontCommand { get; }

    private void ClearMonoGameFont(ProjectFontModel font) {
        font.Definition = font.Definition.WithMonoGameFont(Guid.Empty);
    }

    private void ClearSpriteSheetFont(ProjectFontModel font) {
        font.Definition = font.Definition.WithSpriteSheetFont(Guid.Empty, Guid.Empty);
    }

    private void ResetFontCategories() {
        if (this.Value != null) {
            var categories = Enum.GetValues<FontCategory>().ToList();
            categories.Remove(FontCategory.None);

            var models = categories.Select(category =>
                new ProjectFontModel(new ProjectFontKey(category, this.CurrentCulture), this._assetManager, this._undoService, this.Value));

            this._fontsInCurrentCulture.Reset(models);
        }
    }

    private async Task SelectMonoGameFont(ProjectFontModel font) {
        var contentNode = await this._dialogService.OpenContentSelectionDialog(typeof(SpriteFont), false, this.Title);
        if (contentNode != null && contentNode.Id != Guid.Empty) {
            var contentId = contentNode.Id;
            var originalDefinition = font.Definition;
            var newDefinition = originalDefinition.WithMonoGameFont(contentId);
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
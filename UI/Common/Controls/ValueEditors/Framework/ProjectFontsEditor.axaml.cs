namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using ReactiveUI;
using Unity;

public partial class ProjectFontsEditor : ValueEditorControl<ProjectFonts> {

    public static readonly DirectProperty<ProjectFontsEditor, IReadOnlyCollection<ResourceCulture>> AvailableCulturesProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, IReadOnlyCollection<ResourceCulture>>(
            nameof(AvailableCultures),
            editor => editor.AvailableCultures);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<ProjectFontsEditor, ResourceCulture> CurrentCultureProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ResourceCulture>(
            nameof(CurrentCulture),
            editor => editor.CurrentCulture,
            (editor, value) => editor.CurrentCulture = value);

    public static readonly DirectProperty<ProjectFontsEditor, IReadOnlyCollection<ProjectFontModel>> FontsInCurrentCultureProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, IReadOnlyCollection<ProjectFontModel>>(
            nameof(FontsInCurrentCulture),
            editor => editor.FontsInCurrentCulture);

    public static readonly DirectProperty<ProjectFontsEditor, ICommand> SelectFontCommandProperty =
        AvaloniaProperty.RegisterDirect<ProjectFontsEditor, ICommand>(
            nameof(SelectFontCommand),
            editor => editor.SelectFontCommand);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly ObservableCollectionExtended<ProjectFontModel> _fontsInCurrentCulture = new();
    private readonly IUndoService _undoService;
    private ResourceCulture _currentCulture = ResourceCulture.Default;

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
        this.ClearCommand = ReactiveCommand.Create<ProjectFontModel>(this.Clear);
        this.SelectFontCommand = ReactiveCommand.CreateFromTask<ProjectFontModel>(this.SelectFont);
        this.ResetFontCategories();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<ResourceCulture> AvailableCultures { get; }

    public ICommand ClearCommand { get; }

    public IReadOnlyCollection<ProjectFontModel> FontsInCurrentCulture => this._fontsInCurrentCulture;

    public ICommand SelectFontCommand { get; }

    public ResourceCulture CurrentCulture {
        get => this._currentCulture;
        set {
            if (value != this._currentCulture) {
                this.SetAndRaise(CurrentCultureProperty, ref this._currentCulture, value);
                this.ResetFontCategories();
            }
        }
    }

    private void Clear(ProjectFontModel font) {
        font.Definition = ProjectFontDefinition.Empty;
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

    private async Task SelectFont(ProjectFontModel font) {
        var (spriteSheet, packagedAssetId) = await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteSheetFont>();
        if (spriteSheet != null && packagedAssetId != Guid.Empty) {
            var contentId = spriteSheet.ContentId;
            var originalDefinition = font.Definition;
            var newDefinition = new ProjectFontDefinition(contentId, packagedAssetId);
            this._undoService.Do(
                () => { font.Definition = newDefinition; },
                () => { font.Definition = originalDefinition; });
        }
    }
}
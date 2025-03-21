namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common.Attributes;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class SpriteSheetSpriteIndexReferenceEditor : ValueEditorControl<SpriteSheetSpriteIndexReference> {
    public static readonly DirectProperty<SpriteSheetSpriteIndexReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetSpriteIndexReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<SpriteSheetSpriteIndexReferenceEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetSpriteIndexReferenceEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<SpriteSheetSpriteIndexReferenceEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetSpriteIndexReferenceEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly IUndoService _undoService;

    private string _pathText;

    public SpriteSheetSpriteIndexReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetSpriteIndexReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;

        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.WhenAny(x => x.Value, y => y.Value.SpriteSheetId != Guid.Empty));
        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);

        this.ResetPath();
        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public string PathText {
        get => this._pathText;
        private set => this.SetAndRaise(PathTextProperty, ref this._pathText, value);
    }

    public ICommand SelectCommand { get; }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<SpriteSheetSpriteIndexReference> args) {
        base.OnValueChanged(args);
        this.ResetPath();
    }

    private void Clear() {
        var originalValue = this.Value;

        if (originalValue.SpriteSheetId != Guid.Empty) {
            this._undoService.Do(
                () => this.Value = SpriteSheetSpriteIndexReference.Empty,
                () => this.Value = originalValue);
        }
    }

    private Type ConvertAssetKindToType(SpriteSheetAssetKind assetKind) {
        return assetKind switch {
            SpriteSheetAssetKind.Animation => typeof(SpriteAnimation),
            SpriteSheetAssetKind.AutoTileSet => typeof(AutoTileSet),
            SpriteSheetAssetKind.Font => typeof(SpriteSheetFont),
            SpriteSheetAssetKind.GamePadIconSet => typeof(GamePadIconSet),
            SpriteSheetAssetKind.KeyboardIconSet => typeof(KeyboardIconSet),
            _ => throw new ArgumentOutOfRangeException(nameof(assetKind), assetKind, null)
        };
    }

    private void ResetPath() {
        this.PathText = null;

        if (this._assetManager != null &&
            this.Value.SpriteSheetId != Guid.Empty &&
            this._assetManager.TryGetMetadata(this.Value.SpriteSheetId, out var metadata)) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    private async Task Select() {
        var (spriteSheet, spriteIndex) = await this._dialogService.OpenSpriteSelectionDialog();

        if (spriteSheet != null) {
            var originalValue = this.Value;

            this._undoService.Do(() => { this.Value = new SpriteSheetSpriteIndexReference(spriteSheet.ContentId, spriteIndex); }, () => { this.Value = originalValue; });
        }
    }
}
namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common.Attributes;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class SpriteSheetAssetGuidReferenceEditor : ValueEditorControl<SpriteSheetAssetGuidReference> {
    public static readonly DirectProperty<SpriteSheetAssetGuidReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetAssetGuidReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<SpriteSheetAssetGuidReferenceEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetAssetGuidReferenceEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<SpriteSheetAssetGuidReferenceEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetAssetGuidReferenceEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly SpriteSheetAssetKind _assetKind;

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly IUndoService _undoService;

    private string _pathText;

    public SpriteSheetAssetGuidReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetAssetGuidReferenceEditor(
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


        this._assetKind = SpriteSheetAssetKind.Animation;
        if (dependencies?.Owner?.GetType() is { } ownerType) {
            var members = ownerType.GetMember(dependencies.ValuePropertyName);
            if (members.FirstOrDefault() is { } info && info.GetCustomAttribute<SpriteSheetAssetGuidAttribute>() is { } attribute) {
                this._assetKind = attribute.AssetKind;
            }
        }

        this.ResetPath();
        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public ICommand SelectCommand { get; }

    public string PathText {
        get => this._pathText;
        private set => this.SetAndRaise(PathTextProperty, ref this._pathText, value);
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<SpriteSheetAssetGuidReference> args) {
        base.OnValueChanged(args);
        this.ResetPath();
    }

    private void Clear() {
        var originalValue = this.Value;

        if (originalValue.SpriteSheetId != Guid.Empty) {
            this._undoService.Do(
                () => this.Value = new SpriteSheetAssetGuidReference(),
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
            this.Value.AssetId != Guid.Empty &&
            this._assetManager.TryGetMetadata(this.Value.SpriteSheetId, out var metadata)) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    private async Task Select() {
        var (spriteSheet, packagedAssetId) = this._assetKind switch {
            SpriteSheetAssetKind.Animation => await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteAnimation>(this.Title),
            SpriteSheetAssetKind.AutoTileSet => await this._dialogService.OpenSpriteSheetAssetSelectionDialog<AutoTileSet>(this.Title),
            SpriteSheetAssetKind.Font => await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteSheetFont>(this.Title),
            SpriteSheetAssetKind.GamePadIconSet => await this._dialogService.OpenSpriteSheetAssetSelectionDialog<GamePadIconSet>(this.Title),
            SpriteSheetAssetKind.KeyboardIconSet => await this._dialogService.OpenSpriteSheetAssetSelectionDialog<KeyboardIconSet>(this.Title),
            _ => (null, Guid.Empty)
        };


        if (spriteSheet != null && packagedAssetId != Guid.Empty) {
            var originalValue = this.Value;

            this._undoService.Do(() => { this.Value = new SpriteSheetAssetGuidReference(spriteSheet.ContentId, packagedAssetId); }, () => { this.Value = originalValue; });
        }
    }
}
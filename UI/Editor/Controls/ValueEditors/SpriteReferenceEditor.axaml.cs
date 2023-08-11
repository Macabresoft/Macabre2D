namespace Macabresoft.Macabre2D.UI.Editor;

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public partial class SpriteReferenceEditor : BaseSpriteSheetReferenceEditor<SpriteReference> {
    public static readonly DirectProperty<SpriteReferenceEditor, int> MaxIndexProperty =
        AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, int>(
            nameof(MaxIndex),
            editor => editor.MaxIndex);

    public static readonly DirectProperty<SpriteReferenceEditor, SpriteDisplayModel> SpriteProperty =
        AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, SpriteDisplayModel>(
            nameof(Sprite),
            editor => editor.Sprite);

    private int _maxIndex;
    private SpriteDisplayModel _sprite;

    public SpriteReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
        this.InitializeComponent();
    }

    public BaseSpriteEntity RenderEntity => this.Owner as BaseSpriteEntity;

    public int MaxIndex {
        get => this._maxIndex;
        private set => this.SetAndRaise(MaxIndexProperty, ref this._maxIndex, value);
    }

    public SpriteDisplayModel Sprite {
        get => this._sprite;
        private set => this.SetAndRaise(SpriteProperty, ref this._sprite, value);
    }

    protected override void ResetPath() {
        base.ResetPath();

        this.Sprite = null;
        this.MaxIndex = 0;

        if (this.Bitmap != null && this.Value is { Asset: { } spriteSheet } reference) {
            this.MaxIndex = spriteSheet.MaxIndex;
            this.Sprite = new SpriteDisplayModel(this.Bitmap, reference.SpriteIndex, reference.Asset);
        }
    }

    protected override async Task Select() {
        var (spriteSheet, spriteIndex) = await this.DialogService.OpenSpriteSelectionDialog();
        if (spriteSheet != null) {
            var originalAsset = this.Value.Asset;
            var originalIndex = this.Value.SpriteIndex;
            this.UndoService.Do(
                () =>
                {
                    this.Value.SpriteIndex = spriteIndex;
                    this.Value.LoadAsset(spriteSheet);
                },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.SpriteIndex = originalIndex;
                        this.Value.LoadAsset(originalAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }

    private void NumericUpDown_OnValueChanged(object _, NumericUpDownValueChangedEventArgs e) {
        if (this.Value != null && e.OldValue.HasValue && e.NewValue.HasValue) {
            var oldValue = (byte)e.OldValue.Value;
            var newValue = (byte)e.NewValue.Value;
            if (oldValue != newValue && this.Value.SpriteIndex != newValue) {
                this.UndoService.Do(() => { this.Value.SpriteIndex = newValue; }, () => { this.Value.SpriteIndex = oldValue; });
            }
        }
    }
}
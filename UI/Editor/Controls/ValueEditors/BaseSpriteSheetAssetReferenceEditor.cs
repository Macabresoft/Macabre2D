namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class BaseSpriteSheetAssetReferenceEditor<TAssetReference, TPackagedAsset> : BaseAssetReferenceEditor<TAssetReference, SpriteSheet>
    where TAssetReference : SpriteSheetAssetReference<TPackagedAsset>, IAssetReference<SpriteSheet>
    where TPackagedAsset : SpriteSheetMember {
    public static readonly DirectProperty<BaseSpriteSheetAssetReferenceEditor<TAssetReference, TPackagedAsset>, Bitmap> BitmapProperty =
        AvaloniaProperty.RegisterDirect<BaseSpriteSheetAssetReferenceEditor<TAssetReference, TPackagedAsset>, Bitmap>(
            nameof(Bitmap),
            editor => editor.Bitmap);

    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private Bitmap _bitmap;

    public BaseSpriteSheetAssetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, undoService) {
        this._fileSystem = fileSystem;
        this._pathService = pathService;
    }

    public Bitmap Bitmap {
        get => this._bitmap;
        private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
    }

    protected override void Clear() {
        var asset = this.Value.Asset;
        var packagedAssetId = this.Value.PackagedAssetId;

        if (asset != null) {
            this.UndoService.Do(
                () => this.Value.Clear(),
                () =>
                {
                    this.Value.LoadAsset(asset);
                    this.Value.PackagedAssetId = packagedAssetId;
                });
        }
    }

    protected override void ResetPath() {
        base.ResetPath();

        this.Bitmap = null;

        if (this.AssetManager != null &&
            this.Value?.Asset != null &&
            this.Value.ContentId != Guid.Empty &&
            this.AssetManager.TryGetMetadata(this.Value.ContentId, out var metadata) &&
            metadata != null) {
            var fileName = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
            var fullPath = Path.Combine(this._pathService.ContentDirectoryPath, fileName);

            if (this._fileSystem.DoesFileExist(fullPath)) {
                var bitmap = new Bitmap(fullPath);
                if (bitmap.PixelSize != PixelSize.Empty) {
                    this.Bitmap = bitmap;
                }
            }
        }
    }

    protected override async Task Select() {
        var (spriteSheet, packagedAssetId) = await this.DialogService.OpenSpriteSheetAssetSelectionDialog<TPackagedAsset>();
        if (spriteSheet != null) {
            var originalAsset = this.Value.Asset;
            var originalPackagedAssetId = this.Value.PackagedAssetId;
            this.UndoService.Do(
                () =>
                {
                    this.Value.LoadAsset(spriteSheet);
                    this.Value.PackagedAssetId = packagedAssetId;
                    this.ResetPath();
                },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.PackagedAssetId = originalPackagedAssetId;
                        this.Value.LoadAsset(originalAsset);
                        this.ResetPath();
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }
}
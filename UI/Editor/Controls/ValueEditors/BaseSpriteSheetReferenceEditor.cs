namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class BaseSpriteSheetReferenceEditor<TAssetReference> : BaseAssetReferenceEditor<TAssetReference, SpriteSheet> where TAssetReference : class, IAssetReference<SpriteSheet> {
    public static readonly DirectProperty<BaseSpriteSheetReferenceEditor<TAssetReference>, Bitmap> BitmapProperty =
        AvaloniaProperty.RegisterDirect<BaseSpriteSheetReferenceEditor<TAssetReference>, Bitmap>(
            nameof(Bitmap),
            editor => editor.Bitmap);

    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private Bitmap _bitmap;

    public BaseSpriteSheetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
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

    protected override void ResetPath() {
        base.ResetPath();

        this.Bitmap = null;

        if (this.AssetManager != null &&
            this.Value is { Asset: not null, HasContent: true } &&
            this.AssetManager.TryGetMetadata(this.Value.ContentId, out var metadata)) {
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
}
namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Display collection for a <see cref="SpriteSheetAsset" />.
/// </summary>
/// <typeparam name="TAsset">The type of asset.</typeparam>
public class SpriteSheetAssetDisplayCollection<TAsset> : NotifyPropertyChanged, IReadOnlyCollection<TAsset> where TAsset : SpriteSheetAsset {
    private readonly ContentFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetAssetDisplayCollection{T}" /> class.
    /// </summary>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The file.</param>
    public SpriteSheetAssetDisplayCollection(SpriteSheet spriteSheet, ContentFile file) {
        this.SpriteSheet = spriteSheet;
        this._file = file;
        this.Assets = this.SpriteSheet.GetAssets<TAsset>();

        var fileInfo = new FileInfo(file.GetFullPath());
        if (fileInfo.Exists) {
            this.Image = new Bitmap(fileInfo.FullName);
        }
    }

    /// <summary>
    /// Gets the assets.
    /// </summary>
    public IReadOnlyCollection<TAsset> Assets { get; }

    /// <inheritdoc />
    public int Count => this.Assets.Count;

    /// <summary>
    /// Gets the image.
    /// </summary>
    public IImage Image { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string Name => this._file.Name;

    /// <summary>
    /// Gets the size of the sprite sheet.
    /// </summary>
    public Size Size => this.Image.Size;

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    public SpriteSheet SpriteSheet { get; }

    /// <inheritdoc />
    public IEnumerator<TAsset> GetEnumerator() {
        return this.Assets.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return this.Assets.GetEnumerator();
    }
}
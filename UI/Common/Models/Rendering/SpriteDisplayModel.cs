namespace Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Macabre2D.Framework;
using Point = Microsoft.Xna.Framework.Point;

/// <summary>
/// A model for displaying sprites.
/// </summary>
public sealed class SpriteDisplayModel {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteDisplayModel" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="index">The index of the sprite on the sprite sheet.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    public SpriteDisplayModel(IImage image, byte index, SpriteSheet spriteSheet) {
        this.Index = index;
        this.SpriteSheet = spriteSheet;

        var sizeAndLocation = SpriteSheet.GetSpriteSizeAndLocation(
            new Point((int)image.Size.Width, (int)image.Size.Height),
            this.SpriteSheet.Columns,
            this.SpriteSheet.Rows,
            index);

        this.Size = new PixelSize(sizeAndLocation.Size.X, sizeAndLocation.Size.Y);

        var rect = new PixelRect(
            new PixelPoint(sizeAndLocation.Location.X, sizeAndLocation.Location.Y),
            this.Size);

        this.Bitmap = new CroppedBitmap(image, rect);
    }

    /// <summary>
    /// Gets the bitmap.
    /// </summary>
    public IImage Bitmap { get; }

    /// <summary>
    /// Gets the index of the sprite on its sprite sheet.
    /// </summary>
    public byte Index { get; }

    /// <summary>
    /// Gets the size of the sprite.
    /// </summary>
    public PixelSize Size { get; }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    public SpriteSheet SpriteSheet { get; }
}
namespace Macabresoft.Macabre2D.UI.Common.Models {
    using System;
    using Avalonia;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Point = Microsoft.Xna.Framework.Point;

    /// <summary>
    /// A model for displaying sprites.
    /// </summary>
    public sealed class SpriteDisplayModel : IDisposable {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteDisplayModel" /> class.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="index">The index of the sprite on the sprite sheet.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        public SpriteDisplayModel(Bitmap bitmap, byte index, SpriteSheet spriteSheet) {
            this.Index = index;
            this.SpriteSheet = spriteSheet;

            var sizeAndLocation = SpriteSheet.GetSpriteSizeAndLocation(
                new Point(bitmap.PixelSize.Width, bitmap.PixelSize.Height),
                this.SpriteSheet.Columns,
                this.SpriteSheet.Rows,
                index);

            var rect = new PixelRect(
                new PixelPoint(sizeAndLocation.Location.X, sizeAndLocation.Location.Y),
                new PixelSize(sizeAndLocation.Size.X, sizeAndLocation.Size.Y));

            this.Bitmap = new CroppedBitmap(bitmap, rect);
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        public CroppedBitmap Bitmap { get; }

        /// <summary>
        /// Gets the index of the sprite on its sprite sheet.
        /// </summary>
        public byte Index { get; }
        
        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet SpriteSheet { get; }

        /// <inheritdoc />
        public void Dispose() {
            this.Bitmap?.Dispose();
        }
    }
}
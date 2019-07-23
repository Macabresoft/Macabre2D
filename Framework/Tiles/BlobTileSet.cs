namespace Macabre2D.Framework.Tiles {

    using System;

    /// <summary>
    /// A complete tile set that can auto-generate as it is painted onto a grid.
    /// </summary>
    public sealed class BlobTileSet : BaseIdentifiable, IAutoTileSet {

        /// <inheritdoc/>
        public event EventHandler<CardinalDirection> SpriteChanged;

        /// <inheritdoc/>
        public Sprite GetSprite(CardinalDirection relativeDirectionOfActiveSprites) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetSprite(Sprite sprite, CardinalDirection relativeDirectionOfActiveSprites) {
            throw new NotImplementedException();
        }
    }
}
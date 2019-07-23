namespace Macabre2D.Framework.Tiles {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A tile set that supports block style patterns. Connected sprites will only be quadrilateral shapes.
    /// </summary>
    public sealed class BlockTileSet : BaseIdentifiable, IAutoTileSet {

        private readonly Dictionary<CardinalDirection, CardinalDirection> _cardinalDirectionToValidCardinalDirection = new Dictionary<CardinalDirection, CardinalDirection>() {
        };

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
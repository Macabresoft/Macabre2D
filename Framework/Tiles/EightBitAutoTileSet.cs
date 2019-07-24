namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Represents four directions from a single tile.
    /// </summary>
    [Flags]
    public enum EightBitDirection {
        None = 0,

        NorthWest = 1 << 0,

        North = 1 << 1,

        NorthEast = 1 << 2,

        West = 1 << 3,

        East = 1 << 4,

        SouthWest = 1 << 5,

        South = 1 << 6,

        SouthEast = 1 << 7,

        All = NorthWest | North | NorthEast | West | East | SouthWest | South | SouthEast
    }

    public sealed class EightBitAutoTileSet : BaseIdentifiable {
        private readonly Sprite[] _sprites = new Sprite[48];

        /// <inheritdoc/>
        public event EventHandler<EightBitDirection> SpriteChanged;

        /// <inheritdoc/>
        public Sprite GetSprite(EightBitDirection filledSurroundingTiles) {
            var index = (byte)filledSurroundingTiles;
            return this._sprites[index];
        }

        /// <inheritdoc/>
        public void SetSprite(Sprite sprite, EightBitDirection filledSurroundingTiles) {
            var index = (byte)filledSurroundingTiles;
            this._sprites[index] = sprite;
            this.SpriteChanged.SafeInvoke(this, filledSurroundingTiles);
        }
    }
}
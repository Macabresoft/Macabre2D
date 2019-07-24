namespace Macabre2D.Framework.Tiles {

    using System;

    /// <summary>
    /// Represents the eight cardinal directions. Can be used as flags.
    /// </summary>
    [Flags]
    public enum FourBitDirection {
        None = 0,

        North = 1 << 0,

        West = 1 << 1,

        East = 1 << 2,

        South = 1 << 3,

        All = North | West | East | South
    }

    /// <summary>
    /// A complete tile set that can auto-generate as it is painted onto a grid.
    /// </summary>
    public sealed class FourBitAutoTileSet : BaseIdentifiable {
        private readonly Sprite[] _sprites = new Sprite[16];

        /// <inheritdoc/>
        public event EventHandler<FourBitDirection> SpriteChanged;

        /// <inheritdoc/>
        public Sprite GetSprite(FourBitDirection filledSurroundingTiles) {
            var index = (byte)filledSurroundingTiles;
            return this._sprites[index];
        }

        /// <inheritdoc/>
        public void SetSprite(Sprite sprite, FourBitDirection filledSurroundingTiles) {
            var index = (byte)filledSurroundingTiles;
            this._sprites[index] = sprite;
            this.SpriteChanged.SafeInvoke(this, filledSurroundingTiles);
        }
    }
}
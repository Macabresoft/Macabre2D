namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Represents four directions from a single tile.
    /// </summary>
    [Flags]
    public enum FourWayDirection {
        None = 0,

        North = 1 << 0,

        West = 1 << 1,

        East = 1 << 2,

        South = 1 << 3,

        All = North | West | East | South
    }

    /// <summary>
    /// An automatic tile set that takes in up to four directions to determine a sprite.
    /// </summary>
    public sealed class FourWayAutoTileSet : BaseAutoTileSet {
        private const byte ArrayLength = 16;

        /// <inheritdoc/>
        public override byte Size {
            get {
                return FourWayAutoTileSet.ArrayLength;
            }
        }
    }
}
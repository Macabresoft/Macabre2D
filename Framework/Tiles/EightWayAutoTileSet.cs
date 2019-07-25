namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Represents four directions from a single tile.
    /// </summary>
    [Flags]
    public enum EightWayDirection {
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

    /// <summary>
    /// An automatic tile set that takes in up to eight directions to determine a sprite.
    /// </summary>
    public sealed class EightWayAutoTileSet : BaseAutoTileSet {
        private const byte ArrayLength = 48;

        /// <inheritdoc/>
        public override byte Size {
            get {
                return EightWayAutoTileSet.ArrayLength;
            }
        }
    }
}
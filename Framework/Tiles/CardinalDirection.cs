namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Represents the eight cardinal directions. Can be used as flags.
    /// </summary>
    [Flags]
    public enum CardinalDirection {
        None = 0,

        North = 1 << 0,

        NorthEast = 1 << 1,

        East = 1 << 2,

        SouthEast = 1 << 3,

        South = 1 << 4,

        SouthWest = 1 << 5,

        West = 1 << 6,

        NorthWest = 1 << 7,

        All = ~0
    }
}
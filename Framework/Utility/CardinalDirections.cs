namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// Represents four directions from a single tile.
/// </summary>
[Flags]
public enum CardinalDirections : byte {
    None = 0,
    North = 1 << 0,
    West = 1 << 1,
    East = 1 << 2,
    South = 1 << 3,
    NorthEast = North | East,
    NorthWest = North | West,
    SouthEast = South | East,
    SouthWest = South | West,
    All = North | West | East | South
}
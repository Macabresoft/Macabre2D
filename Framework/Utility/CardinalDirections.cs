namespace Macabre2D.Framework;

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

/// <summary>
/// Extension methods for <see cref="CardinalDirections" />.
/// </summary>
public static class CardinalDirectionsExtensions {
    /// <summary>
    /// Gets the opposite direction if applicable.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns>The direction in the opposite direction if applicable, otherwise <see cref="CardinalDirections.None" />.</returns>
    public static CardinalDirections ToOppositeDirection(this CardinalDirections direction) => direction switch {
        CardinalDirections.West => CardinalDirections.East,
        CardinalDirections.North => CardinalDirections.South,
        CardinalDirections.East => CardinalDirections.West,
        CardinalDirections.South => CardinalDirections.North,
        CardinalDirections.NorthEast => CardinalDirections.SouthWest,
        CardinalDirections.NorthWest => CardinalDirections.SouthEast,
        CardinalDirections.SouthEast => CardinalDirections.NorthWest,
        CardinalDirections.SouthWest => CardinalDirections.NorthEast,
        _ => CardinalDirections.None
    };
}
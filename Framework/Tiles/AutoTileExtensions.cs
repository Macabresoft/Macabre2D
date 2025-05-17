namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Extensions for automatic tile entities.
/// </summary>
public static class AutoTileExtensions {
    /// <summary>
    /// Gets the tile index for an auto tile map given a <see cref="IActiveTileableEntity"/> and a <see cref="tile"/>.
    /// </summary>
    /// <param name="tileableEntity">The tile entity.</param>
    /// <param name="tile">The tile.</param>
    /// <returns>The tile index.</returns>
    public static byte GetStandardAutoTileIndex(this IActiveTileableEntity tileableEntity, Point tile) {
        var direction = CardinalDirections.None;
        if (tileableEntity.HasActiveTileAt(tile + new Point(0, 1))) {
            direction |= CardinalDirections.North;
        }

        if (tileableEntity.HasActiveTileAt(tile + new Point(1, 0))) {
            direction |= CardinalDirections.East;
        }

        if (tileableEntity.HasActiveTileAt(tile - new Point(0, 1))) {
            direction |= CardinalDirections.South;
        }

        if (tileableEntity.HasActiveTileAt(tile - new Point(1, 0))) {
            direction |= CardinalDirections.West;
        }

        return (byte)direction;
    }
}
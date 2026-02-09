namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Interface for entities with active tiles.
/// </summary>
public interface IActiveTileableEntity : IBoundableEntity {
    /// <summary>
    /// Determines whether this instance has an active tile at the provided tile position.
    /// </summary>
    /// <param name="tilePosition">The tile position.</param>
    /// <returns>
    /// <c>true</c> if this instance has a tile at the provided tile position; otherwise, <c>false</c>.
    /// </returns>
    bool HasActiveTileAt(Point tilePosition);

    /// <summary>
    /// Determines whether this instance has an active tile at the provided world position.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <returns>
    /// <c>true</c> if this instance has a tile at the provided world position; otherwise, <c>false</c>.
    /// </returns>
    bool HasActiveTileAt(Vector2 worldPosition);
}
namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// An interface for tileable entities.
/// </summary>
public interface ITileableEntity : IEntity, IBoundable {
    /// <summary>
    /// Occurs when a tile is added or removed.
    /// </summary>
    event EventHandler TilesChanged;

    /// <summary>
    /// Gets the active tiles.
    /// </summary>
    /// <value>The active tiles.</value>
    IReadOnlyCollection<Point> ActiveTiles { get; }

    /// <summary>
    /// Gets the currently used grid.
    /// </summary>
    IGridContainer CurrentGrid { get; }

    /// <summary>
    /// Gets the maximum tile. This represents the maximum X and Y coordinates of the grid,
    /// which may or may not be from the same tile.
    /// </summary>
    /// <value>The maximum tile.</value>
    Point MaximumTile { get; }

    /// <summary>
    /// Gets the minimum tile. This represents the minimum X and Y coordinates of the grid,
    /// which may or may not be from the same tile.
    /// </summary>
    /// <value>The minimum tile.</value>
    Point MinimumTile { get; }

    /// <summary>
    /// Adds the default tile at the specified position.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A value indicating whether or not the tile was added.</returns>
    bool AddTile(Point tile);

    /// <summary>
    /// Clears the tiles.
    /// </summary>
    void ClearTiles();

    /// <summary>
    /// Gets the bounding area for the tile at the specified position.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>The bounding area.</returns>
    BoundingArea GetTileBoundingArea(Point tile);

    /// <summary>
    /// Gets the tile that contains.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <returns>The tile that contains the specified world position.</returns>
    Point GetTileThatContains(Vector2 worldPosition);

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

    /// <summary>
    /// Removes the tile.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>A value indicating whether or not the tile was removed.</returns>
    bool RemoveTile(Point tile);
}
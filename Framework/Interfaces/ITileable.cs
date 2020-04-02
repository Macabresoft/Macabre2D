namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An interface for tileable components.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IBoundable"/>
    public interface ITileable : IBoundable, IWorldTransformable {

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
        /// Gets the grid configuration.
        /// </summary>
        /// <value>The grid configuration.</value>
        GridConfiguration GridConfiguration { get; }

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
        /// Gets the <see cref="LocalGrid"/> transformed to world coordinates.
        /// </summary>
        /// <value>The <see cref="LocalGrid"/> transformed to world coordinates.</value>
        TileGrid WorldGrid { get; }

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
}
namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// An interface for tileable components.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IBoundable"/>
    public interface ITileable : IBoundable {

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        TileGrid LocalGrid { get; }

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

    /// <summary>
    /// An interface for tileable components of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the tile.</typeparam>
    public interface ITileable<T> : ITileable {

        /// <summary>
        /// Gets the available tiles.
        /// </summary>
        /// <value>The available tiles.</value>
        IEnumerable<T> AvailableTiles { get; }

        /// <summary>
        /// Gets the default tile.
        /// </summary>
        /// <value>The default tile.</value>
        T DefaultTile { get; }

        /// <summary>
        /// Sets the default tile.
        /// </summary>
        /// <param name="newDefault">The new default.</param>
        /// <returns>A value indicating whether or not the default tile was changed.</returns>
        bool SetDefaultTile(T newDefault);
    }
}
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
        TileGrid Grid { get; }

        /// Adds the default tile at the specified position. </summary> <param name="position">The position.</param>
        void AddTile(Point position);

        /// <summary>
        /// Removes the tile.
        /// </summary>
        /// <param name="position">The position.</param>
        void RemoveTile(Point position);
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
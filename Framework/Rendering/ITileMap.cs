namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface for tile maps.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IBoundable"/>
    public interface ITileMap : IBoundable {

        /// <summary>
        /// Gets the size of the map.
        /// </summary>
        /// <value>The size of the map.</value>
        Point MapSize { get; }

        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        Point TileSize { get; }
    }
}
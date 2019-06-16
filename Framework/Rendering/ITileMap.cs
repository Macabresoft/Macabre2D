namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface for tile maps.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IBoundable"/>
    public interface ITileMap : IBoundable {

        /// <summary>
        /// Gets or sets the size of the map. This is how many grid tiles it will contain.
        /// </summary>
        /// <value>The size of the map.</value>
        Point MapSize { get; }

        /// <summary>
        /// Gets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        Point TileSize { get; }
    }
}
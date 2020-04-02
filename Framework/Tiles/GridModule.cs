namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module that stores a <see cref="TileGrid"/> that many components can access and use.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseModule"/>
    public sealed class GridModule : BaseModule {

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        [DataMember]
        public TileGrid Grid { get; set; } = new TileGrid(Vector2.One);
    }
}
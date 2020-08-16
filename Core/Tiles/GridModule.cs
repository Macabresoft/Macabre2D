namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module that stores a <see cref="TileGrid"/> that many components can access and use.
    /// </summary>
    public sealed class GridModule : BaseModule {
        private TileGrid _grid = new TileGrid(Vector2.One);

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        [DataMember]
        public TileGrid Grid {
            get {
                return this._grid;
            }

            set {
                this.Set(ref this._grid, value, true);
            }
        }
    }
}
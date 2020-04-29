namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module that stores a <see cref="TileGrid"/> that many components can access and use.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseModule"/>
    public sealed class GridModule : BaseModule {
        private TileGrid _grid = new TileGrid(Vector2.One);

        /// <summary>
        /// Occurs when the grid changes.
        /// </summary>
        public event EventHandler GridChanged;

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
                if (this._grid != value) {
                    this._grid = value;
                    this.GridChanged.SafeInvoke(this);
                }
            }
        }
    }
}
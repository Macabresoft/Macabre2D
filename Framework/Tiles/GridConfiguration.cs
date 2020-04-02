namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The configuration of a <see cref="TileGrid"/> for s <see cref="TileableComponent"/>.
    /// </summary>
    [DataContract]
    public sealed class GridConfiguration {
#pragma warning disable IDE0044 // Add readonly modifier

        [DataMember(Name = "Grid Module", Order = 1)]
        private GridModule _gridModule;

#pragma warning restore IDE0044 // Add readonly modifier

        private TileGrid _localGrid = new TileGrid(Vector2.One);
        private bool _useLocalGrid = true;

        /// <summary>
        /// Occurs when the grid changes.
        /// </summary>
        public event EventHandler GridChanged;

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public TileGrid Grid {
            get {
                return (this.UseLocalGrid || this._gridModule == null) ? this._localGrid : this._gridModule.Grid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance should use the local grid.
        /// </summary>
        /// <value><c>true</c> if this instance should use the local grid; otherwise, <c>false</c>.</value>
        [DataMember(Name = "Use Local Grid", Order = 0)]
        public bool UseLocalGrid {
            get {
                return this._useLocalGrid;
            }

            set {
                if (value != this._useLocalGrid) {
                    this._useLocalGrid = value;
                    this.GridChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the local grid.
        /// </summary>
        /// <value>The local grid.</value>
        [DataMember(Name = "Local Grid", Order = 1)]
        internal TileGrid LocalGrid {
            get {
                return this._localGrid;
            }

            set {
                if (this._localGrid != value) {
                    this._localGrid = value;
                    this.GridChanged.SafeInvoke(this);
                }
            }
        }
    }
}
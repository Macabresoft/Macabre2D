namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The configuration of a <see cref="TileGrid"/> for s <see cref="TileableComponent"/>.
    /// </summary>
    [DataContract]
    public sealed class GridConfiguration {
        private GridModule _gridModule;
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
        /// Gets or sets the grid module. This will be used when <see cref="UseLocalGrid"/> is set
        /// to false.
        /// </summary>
        /// <value>The grid module.</value>
        [DataMember(Name = "Grid Module", Order = 1)]
        public GridModule GridModule {
            get {
                return this._gridModule;
            }

            set {
                var oldGridModule = this._gridModule;

                if (this._gridModule != value) {
                    this._gridModule = value;

                    if (!this.UseLocalGrid) {
                        this.GridChanged.SafeInvoke(this);
                    }

                    if (this._gridModule != null) {
                        this._gridModule.GridChanged += this.GridModule_GridChanged;
                    }

                    if (oldGridModule != null) {
                        oldGridModule.GridChanged -= this.GridModule_GridChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the local grid.
        /// </summary>
        /// <value>The local grid.</value>
        [DataMember(Name = "Local Grid", Order = 1)]
        public TileGrid LocalGrid {
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

        private void GridModule_GridChanged(object sender, EventArgs e) {
            if (!this.UseLocalGrid) {
                this.GridChanged.SafeInvoke(this);
            }
        }
    }
}
namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// The configuration of a <see cref="TileGrid"/> for s <see cref="TileableComponent"/>.
    /// </summary>
    [DataContract]
    public sealed class GridConfiguration : NotifyPropertyChanged {
        private GridModule _gridModule;
        private TileGrid _localGrid = new TileGrid(Vector2.One);
        private bool _useLocalGrid = true;

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
                        this.RaisePropertyChanged(true, nameof(this.Grid));
                    }

                    if (this._gridModule != null) {
                        this._gridModule.PropertyChanged += this.GridModule_PropertyChanged;
                    }

                    if (oldGridModule != null) {
                        oldGridModule.PropertyChanged -= this.GridModule_PropertyChanged;
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
                if (this.Set(ref this._localGrid, value) && this.UseLocalGrid) {
                    this.RaisePropertyChanged(true, nameof(this.Grid));
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
                if (this.Set(ref this._useLocalGrid, value)) {
                    this.RaisePropertyChanged(true, nameof(this.Grid));
                }
            }
        }

        private void GridModule_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!this.UseLocalGrid && e.PropertyName == nameof(this.GridModule.Grid)) {
                this.RaisePropertyChanged(true, nameof(this.Grid));
            }
        }
    }
}
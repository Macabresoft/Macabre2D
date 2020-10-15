using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Macabresoft.Macabre2D.Framework.Tiles {

    /// <summary>
    /// A component that holds a grid.
    /// </summary>
    [DataContract]
    public sealed class GridComponent : GameComponent {
        private TileGrid _grid = TileGrid.Empty;
        private TileGrid _worldGrid = TileGrid.Empty;

        /// <inheritdoc />
        [DataMember]
        public TileGrid Grid {
            get {
                return this._grid;
            }

            set {
                if (this.Set(ref this._grid, value)) {
                    this.ResetWorldGrid();
                }
            }
        }

        /// <inheritdoc />
        public TileGrid WorldGrid {
            get {
                return this._worldGrid;
            }

            private set {
                this.Set(ref this._worldGrid, value);
            }
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.ResetWorldGrid();
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            base.OnEntityPropertyChanged(e);

            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.ResetWorldGrid();
            }
        }

        private TileGrid CreateWorldGrid() {
            var transform = this.Entity.Transform;

            var matrix =
                Matrix.CreateScale(transform.Scale.X, transform.Scale.Y, 1f) *
                Matrix.CreateScale(this.Grid.TileSize.X, this.Grid.TileSize.Y, 1f) *
                Matrix.CreateTranslation(this.Grid.Offset.X, this.Grid.Offset.Y, 0f) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            var gridTransform = matrix.ToTransform();
            var tileGrid = new TileGrid(gridTransform.Scale, gridTransform.Position);

            return tileGrid;
        }

        private void ResetWorldGrid() {
            this._worldGrid = this.CreateWorldGrid();
            this.RaisePropertyChanged(nameof(this.WorldGrid));
        }
    }
}
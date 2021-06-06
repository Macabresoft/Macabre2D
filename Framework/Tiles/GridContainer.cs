namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface for an entity which contains a grid.
    /// </summary>
    public interface IGridContainer : IEntity {
        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        TileGrid Grid { get; set; }

        /// <summary>
        /// Gets the nearest tile position to the provided position.
        /// </summary>
        /// <param name="position">The position of which to find the nearest tile.</param>
        /// <returns>The nearest tile position in world space.</returns>
        Vector2 GetNearestTilePosition(Vector2 position);
    }

    /// <summary>
    /// An entity which contains a grid.
    /// </summary>
    public class GridContainer : Entity, IGridContainer {
        /// <summary>
        /// An empty grid container. Defaults to a 1 by 1 grid.
        /// </summary>
        public static readonly IGridContainer EmptyGridContainer = new EmptyGridContainer();

        private TileGrid _grid = TileGrid.One;

        /// <inheritdoc />
        [DataMember]
        public TileGrid Grid {
            get => this._grid;
            set => this.Set(ref this._grid, value);
        }

        /// <inheritdoc />
        public Vector2 GetNearestTilePosition(Vector2 position) {
            var x = position.X;
            var y = position.Y;

            if (this.Grid.TileSize.X > 0f) {
                x = (float)Math.Round(x / this.Grid.TileSize.X) * this.Grid.TileSize.X;
            }

            if (this.Grid.TileSize.Y > 0f) {
                y = (float)Math.Round(y / this.Grid.TileSize.Y) * this.Grid.TileSize.Y;
            }

            return new Vector2(x, y);
        }
    }

    internal class EmptyGridContainer : Entity.EmptyEntity, IGridContainer {
        public TileGrid Grid {
            get => TileGrid.One;
            set { }
        }

        public Vector2 GetNearestTilePosition(Vector2 position) {
            throw new NotImplementedException();
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;

    /// <summary>
    /// An interface for an entity which contains a grid.
    /// </summary>
    public interface IGridContainer : IGameEntity {
        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        TileGrid Grid { get; set; }
    }

    /// <summary>
    /// An entity which contains a grid.
    /// </summary>
    public class GridContainer : GameEntity, IGridContainer {
        /// <summary>
        /// An empty grid container. Defaults to a 1 by 1 grid.
        /// </summary>
        public static readonly IGridContainer Empty = new EmptyGridContainer();

        private TileGrid _grid = TileGrid.One;

        /// <inheritdoc />
        [DataMember]
        public TileGrid Grid {
            get => this._grid;
            set => this.Set(ref this._grid, value);
        }
    }

    internal class EmptyGridContainer : GameEntity.EmptyGameEntity, IGridContainer {
        public TileGrid Grid {
            get => TileGrid.One;
            set { }
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A grid of uniform tiles.
    /// </summary>
    [DataContract]
    [Category("Grid")]
    public readonly struct TileGrid {
        /// <summary>
        /// An empty tile grid.
        /// </summary>
        public static readonly TileGrid One = new(Vector2.One);

        /// <summary>
        /// Gets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        [DataMember(Order = 0, Name = "Tile Size")]
        public readonly Vector2 TileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid" /> struct.
        /// </summary>
        /// <param name="xTileSize">The size of each tile on the x-axis.</param>
        /// <param name="yTileSize">The size of each tile on the y-axis.</param>
        public TileGrid(float xTileSize, float yTileSize) : this(new Vector2(xTileSize, yTileSize)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid" /> struct.
        /// </summary>
        /// <param name="tileSize">Size of the tile.</param>
        public TileGrid(Vector2 tileSize) {
            this.TileSize = tileSize;
        }

        /// <inheritdoc cref="object" />
        public static bool operator !=(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize != grid2.TileSize;
        }

        /// <inheritdoc cref="object" />
        public static bool operator ==(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize == grid2.TileSize;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) {
            return obj is TileGrid grid &&
                   this.TileSize.Equals(grid.TileSize);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TileSize.GetHashCode();
        }

        /// <summary>
        /// Gets the tile position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The tile position.</returns>
        public Vector2 GetTilePosition(Point tile) {
            return new(tile.X * this.TileSize.X, tile.Y * this.TileSize.Y);
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"Tile Size: ({this.TileSize.X}, {this.TileSize.Y})";
        }
    }
}
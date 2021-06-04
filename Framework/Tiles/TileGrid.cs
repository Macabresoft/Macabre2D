namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using System.ComponentModel;

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
        /// The offset
        /// </summary>
        [DataMember(Order = 1)]
        public readonly Vector2 Offset;

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
        public TileGrid(float xTileSize, float yTileSize) : this(new Vector2(xTileSize, yTileSize), Vector2.Zero) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid" /> struct.
        /// </summary>
        /// <param name="tileSize">Size of the tile.</param>
        public TileGrid(Vector2 tileSize) : this(tileSize, Vector2.Zero) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid" /> struct.
        /// </summary>
        /// <param name="tileSize">Size of the tile.</param>
        /// <param name="offset">The offset of the grid from an entity's origin.</param>
        public TileGrid(Vector2 tileSize, Vector2 offset) {
            this.Offset = offset;
            this.TileSize = tileSize;
        }

        /// <inheritdoc cref="object" />
        public static bool operator !=(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize != grid2.TileSize || grid1.Offset != grid2.Offset;
        }

        /// <inheritdoc cref="object" />
        public static bool operator ==(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize == grid2.TileSize && grid1.Offset == grid2.Offset;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) {
            return obj is TileGrid grid &&
                   this.Offset.Equals(grid.Offset) &&
                   this.TileSize.Equals(grid.TileSize);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(this.Offset, this.TileSize);
        }

        /// <summary>
        /// Gets the tile position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The tile position.</returns>
        public Vector2 GetTilePosition(Point tile) {
            return new Vector2(tile.X * this.TileSize.X, tile.Y * this.TileSize.Y) + this.Offset;
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"Tile Size: ({this.TileSize.X}, {this.TileSize.Y}), Offset: ({this.Offset.X}, {this.Offset.Y})";
        }
    }
}
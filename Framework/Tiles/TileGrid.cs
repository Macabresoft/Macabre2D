namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A grid of uniform tiless.
    /// </summary>
    [DataContract]
    public struct TileGrid {

        /// <summary>
        /// The offset
        /// </summary>
        [DataMember]
        public readonly Vector2 Offset;

        /// <summary>
        /// Gets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        [DataMember]
        public readonly Vector2 TileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid"/> struct.
        /// </summary>
        /// <param name="tileSize">Size of the tile.</param>
        public TileGrid(Vector2 tileSize) : this(tileSize, Vector2.Zero) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGrid"/> struct.
        /// </summary>
        /// <param name="gridSize">Size of the grid.</param>
        /// <param name="tileSize">Size of the tile.</param>
        public TileGrid(Vector2 tileSize, Vector2 offset) {
            this.Offset = offset;
            this.TileSize = tileSize;
        }

        /// <inheritdoc/>
        public static bool operator !=(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize != grid2.TileSize || grid1.Offset != grid2.Offset;
        }

        /// <inheritdoc/>
        public static bool operator ==(TileGrid grid1, TileGrid grid2) {
            return grid1.TileSize == grid2.TileSize && grid1.Offset == grid2.Offset;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            return obj is TileGrid grid &&
                   this.Offset.Equals(grid.Offset) &&
                   this.TileSize.Equals(grid.TileSize);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            var hashCode = 560318066;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Offset);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.TileSize);
            return hashCode;
        }
    }
}
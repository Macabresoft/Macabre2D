namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile set that will display a random single tile in every active location in the tile map.
    /// </summary>
    public sealed class RandomTileSet : BaseSpriteSheetAsset {
        [DataMember]
        private readonly List<WeightedTile> _tiles = new();

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        public IReadOnlyCollection<WeightedTile> Tiles => this._tiles;

        /// <summary>
        /// Adds the tile.
        /// </summary>
        /// <returns>An empty <see cref="WeightedTile" />.</returns>
        public WeightedTile AddTile() {
            var tile = new WeightedTile();
            this._tiles.Add(tile);
            return tile;
        }

        /// <summary>
        /// Gets the random index.
        /// </summary>
        /// <returns>A valid random index.</returns>
        public ushort GetRandomIndex() {
            var index = (ushort)0;

            if (this._tiles.Count > 1) {
                var total = this._tiles.Sum(x => x.Weight);
                var random = new Random();
                var randomValue = random.Next(1, total);
                var count = (int)this._tiles[0].Weight;

                while (count < randomValue && index < this._tiles.Count) {
                    count += this._tiles[index].Weight;
                    index++;
                }
            }

            return index;
        }

        /// <summary>
        /// Gets the sprite at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The sprite at the specified index.</returns>
        public byte GetSpriteIndex(ushort index) {
            return index < this._tiles.Count ? this._tiles[index].SpriteIndex : (byte)(this._tiles[index].SpriteIndex % this._tiles.Count);
        }

        /// <summary>
        /// Removes the tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A value indicating whether or not the tile was removed.</returns>
        public bool RemoveTile(WeightedTile tile) {
            return this._tiles.Remove(tile);
        }
    }
}
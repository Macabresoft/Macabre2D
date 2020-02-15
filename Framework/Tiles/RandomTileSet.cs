namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile set that will display a random single tile in every active location in the tile map.
    /// </summary>
    public sealed class RandomTileSet : BaseIdentifiable, IAsset {

        [DataMember]
        private readonly List<WeightedTile> _tiles = new List<WeightedTile>();

        private bool _isLoaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomTileSet"/> class.
        /// </summary>
        public RandomTileSet() {
        }

        /// <summary>
        /// Occurs when a sprite changes for a particular index.
        /// </summary>
        public event EventHandler<ushort> SpriteChanged;

        /// <inheritdoc/>
        [DataMember]
        public Guid AssetId { get; set; }

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        public IReadOnlyCollection<WeightedTile> Tiles {
            get {
                return this._tiles;
            }
        }

        /// <summary>
        /// Adds the tile.
        /// </summary>
        /// <returns>An empty <see cref="WeightedTile"/>.</returns>
        public WeightedTile AddTile() {
            var tile = new WeightedTile();
            this._tiles.Add(tile);
            tile.SpriteChanged += this.Tile_SpriteChanged;
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
        public Sprite GetSprite(ushort index) {
            return index < this._tiles.Count ? this._tiles[index]?.Sprite : null;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load() {
            try {
                foreach (var tile in this._tiles) {
                    tile?.Sprite?.Load();
                }
            }
            finally {
                this._isLoaded = true;
            }
        }

        /// <summary>
        /// Removes the tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A value indicating whether or not the tile was removed.</returns>
        public bool RemoveTile(WeightedTile tile) {
            tile.SpriteChanged -= Tile_SpriteChanged;
            return this._tiles.Remove(tile);
        }

        internal IEnumerable<Guid> GetSpriteIds() {
            return this._tiles.Where(x => x.Sprite != null).Select(x => x.Sprite.Id).ToList();
        }

        internal bool HasSprite(Guid spriteId) {
            return this._tiles.Any(x => x?.Sprite?.Id == spriteId);
        }

        internal void RefreshSprite(Sprite sprite) {
            if (sprite != null) {
                var tilesForRefresh = this._tiles.Where(x => x.Sprite?.Id == sprite.Id).ToList();
                foreach (var tile in tilesForRefresh) {
                    tile.Sprite = sprite;
                }
            }
        }

        internal bool RemoveSprite(Guid spriteId) {
            var result = false;
            var tiles = this._tiles.Where(x => x.Sprite?.Id == spriteId).ToList();

            foreach (var tile in tiles) {
                tile.Sprite = null;
            }

            return result;
        }

        internal bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this.Tiles.Select(x => x.Sprite).FirstOrDefault(x => x?.Id == spriteId);
            return sprite != null;
        }

        private void Tile_SpriteChanged(object sender, EventArgs e) {
            if (this._isLoaded && sender is WeightedTile tile) {
                tile.Sprite?.Load();
                var index = (ushort)this._tiles.IndexOf(tile);
                this.SpriteChanged?.SafeInvoke(this, index);
            }
        }
    }
}
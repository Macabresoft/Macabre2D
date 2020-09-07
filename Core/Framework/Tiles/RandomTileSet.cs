namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile set that will display a random single tile in every active location in the tile map.
    /// </summary>
    public sealed class RandomTileSet : BaseIdentifiable, IAsset {

        /// <summary>
        /// An empty random tile set.
        /// </summary>
        public static readonly RandomTileSet Empty = new RandomTileSet() {
            Name = "Empty"
        };

        [DataMember]
        private readonly List<WeightedTile> _tiles = new List<WeightedTile>();

        private bool _isLoaded = false;
        private string _name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomTileSet" /> class.
        /// </summary>
        public RandomTileSet() {
        }

        /// <summary>
        /// Occurs when a sprite changes for a particular index.
        /// </summary>
        public event EventHandler<ushort>? SpriteChanged;

        /// <inheritdoc />
        [DataMember]
        public Guid AssetId { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

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
        /// <returns>An empty <see cref="WeightedTile" />.</returns>
        public WeightedTile AddTile() {
            var tile = new WeightedTile();
            this._tiles.Add(tile);
            tile.PropertyChanged += this.Tile_PropertyChanged;
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
            return index < this._tiles.Count ? this._tiles[index].Sprite : Sprite.Empty;
        }

        /// <summary>
        /// Gets the sprite ids.
        /// </summary>
        /// <returns>The sprite identifiers.</returns>
        public IEnumerable<Guid> GetSpriteIds() {
            return this._tiles.Where(x => x.Sprite != null).Select(x => x.Sprite.Id).ToList();
        }

        /// <summary>
        /// Determines whether the specified sprite identifier has sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <returns><c>true</c> if the specified sprite identifier has sprite; otherwise, <c>false</c>.</returns>
        public bool HasSprite(Guid spriteId) {
            return this._tiles.Any(x => x?.Sprite?.Id == spriteId);
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
        /// Refreshes the sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void RefreshSprite(Sprite sprite) {
            if (sprite != null) {
                var tilesForRefresh = this._tiles.Where(x => x.Sprite?.Id == sprite.Id).ToList();
                foreach (var tile in tilesForRefresh) {
                    tile.Sprite = sprite;
                }
            }
        }

        /// <summary>
        /// Removes the sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <returns>A value indicating whether or not the sprite was successfully removed.</returns>
        public bool RemoveSprite(Guid spriteId) {
            var result = false;
            var tiles = this._tiles.Where(x => x.Sprite?.Id == spriteId).ToList();

            foreach (var tile in tiles) {
                tile.Sprite = Sprite.Empty;
            }

            return result;
        }

        /// <summary>
        /// Removes the tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A value indicating whether or not the tile was removed.</returns>
        public bool RemoveTile(WeightedTile tile) {
            tile.PropertyChanged -= Tile_PropertyChanged;
            return this._tiles.Remove(tile);
        }

        /// <summary>
        /// Tries the get sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <param name="sprite">The sprite.</param>
        /// <returns>A value indicating whether or not the value was found.</returns>
        public bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this.Tiles.Select(x => x.Sprite).FirstOrDefault(x => x?.Id == spriteId);
            return sprite != null;
        }

        private void Tile_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (this._isLoaded && e.PropertyName == nameof(WeightedTile.Sprite) && sender is WeightedTile tile) {
                tile.Sprite?.Load();
                var index = (ushort)this._tiles.IndexOf(tile);
                this.SpriteChanged?.SafeInvoke(this, index);
            }
        }
    }
}
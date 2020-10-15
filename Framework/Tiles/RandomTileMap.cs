namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile map that chooses a random tile from the provided collection to display in each active location.
    /// </summary>
    public sealed class RandomTileMap : RenderableTileMap, IAssetComponent<RandomTileSet>, IAssetComponent<Sprite> {

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Point, ushort> _activeTileToIndex = new Dictionary<Point, ushort>();

        private Color _color = Color.White;
        private Vector2 _previousWorldScale;
        private Vector2[] _spriteScales = new Vector2[0];
        private RandomTileSet? _tileSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomTileMap" /> class.
        /// </summary>
        public RandomTileMap() : base() {
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Point> ActiveTiles {
            get {
                return this._activeTileToIndex.Keys;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color {
            get {
                return this._color;
            }

            set {
                this.Set(ref this._color, value);
            }
        }

        /// <summary>
        /// Gets or sets the tile set.
        /// </summary>
        /// <value>The tile set.</value>
        [DataMember(Order = 0, Name = "Tile Set")]
        public RandomTileSet? TileSet {
            get {
                return this._tileSet;
            }

            set {
                if (this._tileSet != null) {
                    this._tileSet.SpriteChanged -= this.TileSet_SpriteChanged;
                }

                if (this.Set(ref this._tileSet, value)) {
                    this.LoadTileSet();
                }

                if (this._tileSet != null) {
                    this._tileSet.SpriteChanged += this.TileSet_SpriteChanged;
                }
            }
        }

        /// <inheritdoc />
        IEnumerable<Guid> IAssetComponent<RandomTileSet>.GetOwnedAssetIds() {
            return this.TileSet == null ? new Guid[0] : new[] { this.TileSet.Id };
        }

        /// <inheritdoc />
        IEnumerable<Guid> IAssetComponent<Sprite>.GetOwnedAssetIds() {
            return this.TileSet == null ? Enumerable.Empty<Guid>() : this.TileSet.GetSpriteIds();
        }

        /// <inheritdoc />
        public override bool HasActiveTileAt(Point tilePosition) {
            return this._activeTileToIndex.ContainsKey(tilePosition);
        }

        public bool HasAsset(Guid id) {
            return this.TileSet?.Id == id || this.TileSet?.HasSprite(id) == true;
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this._previousWorldScale = this.Entity.Transform.Scale;
            this.LoadTileSet();
        }

        /// <inheritdoc />
        void IAssetComponent<RandomTileSet>.RefreshAsset(RandomTileSet newInstance) {
            if (newInstance != null && this.TileSet?.Id == newInstance.Id) {
                this.TileSet = newInstance;
            }
        }

        /// <inheritdoc />
        void IAssetComponent<Sprite>.RefreshAsset(Sprite newInstance) {
            this.TileSet?.RefreshSprite(newInstance);
        }

        /// <inheritdoc />
        public bool RemoveAsset(Guid id) {
            var result = false;
            if (this.TileSet != null) {
                if (this.TileSet.Id == id) {
                    this.TileSet = null;
                    result = true;
                }
                else {
                    result = this.TileSet.RemoveSprite(id);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            foreach (var activeTileToIndex in this._activeTileToIndex) {
                var boundingArea = this.GetTileBoundingArea(activeTileToIndex.Key);
                if (boundingArea.Overlaps(viewBoundingArea)) {
                    if (this.TileSet?.GetSprite(activeTileToIndex.Value) is Sprite sprite) {
                        this.Entity.Scene.Game.SpriteBatch?.Draw(sprite, boundingArea.Minimum, this._spriteScales[activeTileToIndex.Value], this.Color);
                    }
                }
            }
        }

        /// <inheritdoc />
        bool IAssetComponent<RandomTileSet>.TryGetAsset(Guid id, out RandomTileSet? asset) {
            var result = false;
            if (this.TileSet?.Id == id) {
                asset = this.TileSet;
                result = true;
            }
            else {
                asset = null;
            }

            return result;
        }

        /// <inheritdoc />
        bool IAssetComponent<Sprite>.TryGetAsset(Guid id, out Sprite? asset) {
            if (this.TileSet != null) {
                this.TileSet.TryGetSprite(id, out asset);
            }
            else {
                asset = null;
            }

            return asset != null;
        }

        /// <inheritdoc />
        protected override void ClearActiveTiles() {
            this._activeTileToIndex.Clear();
        }

        /// <inheritdoc />
        protected override Point GetMaximumTile() {
            return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Max(), this._activeTileToIndex.Keys.Select(t => t.Y).Max());
        }

        /// <inheritdoc />
        protected override Point GetMinimumTile() {
            return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Min(), this._activeTileToIndex.Keys.Select(t => t.Y).Min());
        }

        /// <inheritdoc />
        protected override bool HasActiveTiles() {
            return this._activeTileToIndex.Any();
        }

        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Entity.Transform) && this.Entity.Transform.Scale != this._previousWorldScale) {
                this._previousWorldScale = this.Entity.Transform.Scale;
                this.ResetSpriteScales();
            }
        }

        /// <inheritdoc />
        protected override bool TryAddTile(Point tile) {
            var result = false;
            if (!this._activeTileToIndex.ContainsKey(tile)) {
                result = true;
                this._activeTileToIndex[tile] = this.GetIndex(tile);
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool TryRemoveTile(Point tile) {
            var result = this._activeTileToIndex.Remove(tile);
            return result;
        }

        private ushort GetIndex(Point tile) {
            var index = (ushort)0;
            if (this.TileSet != null && this._activeTileToIndex.TryGetValue(tile, out index)) {
                if (index > this.TileSet.Tiles.Count) {
                    index = this.TileSet.GetRandomIndex();
                }
            }

            return index;
        }

        private void LoadTileSet() {
            this.TileSet?.Load();
            this.ReevaluateIndexes();
            this.ResetSpriteScales();
        }

        private void ReevaluateIndex(Point tile) {
            if (this._activeTileToIndex.ContainsKey(tile)) {
                this._activeTileToIndex[tile] = this.GetIndex(tile);
            }
        }

        private void ReevaluateIndexes() {
            var clonedActiveTiles = this._activeTileToIndex.Keys.ToList();
            foreach (var activeTile in clonedActiveTiles) {
                this.ReevaluateIndex(activeTile);
            }
        }

        private void ResetSpriteScales() {
            if (this.TileSet != null) {
                this._spriteScales = new Vector2[this.TileSet.Tiles.Count];

                for (byte i = 0; i < this._spriteScales.Length; i++) {
                    var sprite = this.TileSet.GetSprite(i);
                    this._spriteScales[i] = this.GetTileScale(sprite);
                }
            }
        }

        private void TileSet_SpriteChanged(object? sender, ushort e) {
            var sprite = this.TileSet?.GetSprite(e);
            this._spriteScales[e] = this.GetTileScale(sprite);
        }
    }
}
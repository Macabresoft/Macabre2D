namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which maps <see cref="IAutoTileSet"/> onto a <see cref="TileGrid"/>.
    /// </summary>
    public sealed class AutoTileMapComponent : TileableComponent, IAssetComponent<AutoTileSet>, IAssetComponent<Sprite>, IDrawableComponent {

        [DataMember]
        private readonly Dictionary<Point, byte> _activeTileToIndex = new Dictionary<Point, byte>();

        private Vector2[] _spriteScales = new Vector2[0];
        private AutoTileSet _tileSet;

        /// <summary>
        /// Represents four directions from a single tile.
        /// </summary>
        [Flags]
        private enum CardinalDirections : byte {
            None = 0,

            North = 1 << 0,

            West = 1 << 1,

            East = 1 << 2,

            South = 1 << 3,

            All = North | West | East | South
        }

        /// <summary>
        /// Represents four directions from a single tile.
        /// </summary>
        [Flags]
        private enum IntermediateDirections : byte {
            None = 0,

            NorthWest = 1 << 0,

            North = 1 << 1,

            NorthEast = 1 << 2,

            West = 1 << 3,

            East = 1 << 4,

            SouthWest = 1 << 5,

            South = 1 << 6,

            SouthEast = 1 << 7,

            All = NorthWest | North | NorthEast | West | East | SouthWest | South | SouthEast
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the tile set.
        /// </summary>
        /// <value>The tile set.</value>
        [DataMember]
        public AutoTileSet TileSet {
            get {
                return this._tileSet;
            }

            set {
                if (this._tileSet?.Id != value?.Id) {
                    var originalValue = this._tileSet;
                    this._tileSet = value;

                    if (this.IsInitialized) {
                        this.LoadContent();

                        if (this._tileSet != null) {
                            this._tileSet.SpriteChanged += this.TileSet_SpriteChanged;
                        }

                        if (originalValue != null) {
                            originalValue.SpriteChanged -= this.TileSet_SpriteChanged;
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            if (this.TileSet != null) {
                foreach (var activeTileToIndex in this._activeTileToIndex) {
                    var boundingArea = this.GetTileBoundingArea(activeTileToIndex.Key);
                    if (boundingArea.Overlaps(viewBoundingArea)) {
                        if (this.TileSet.GetSprite(activeTileToIndex.Value) is Sprite sprite) {
                            MacabreGame.Instance.SpriteBatch.Draw(sprite, boundingArea.Minimum, this._spriteScales[activeTileToIndex.Value], this.Color);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        IEnumerable<Guid> IAssetComponent<AutoTileSet>.GetOwnedAssetIds() {
            return this.TileSet == null ? new Guid[0] : new[] { this.TileSet.Id };
        }

        /// <inheritdoc/>
        IEnumerable<Guid> IAssetComponent<Sprite>.GetOwnedAssetIds() {
            return this.TileSet == null ? Enumerable.Empty<Guid>() : this.TileSet.GetSpriteIds();
        }

        /// <inheritdoc/>
        public override bool HasActiveTileAt(Point tilePosition) {
            return this._activeTileToIndex.ContainsKey(tilePosition);
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this.TileSet?.Id == id || this.TileSet?.HasSprite(id) == true;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized) {
                this.TileSet?.LoadContent();
                this.ReevaluateIndexes();
                this.ResetSpriteScales();
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        void IAssetComponent<AutoTileSet>.RefreshAsset(AutoTileSet newInstance) {
            if (newInstance != null && this.TileSet?.Id == newInstance.Id) {
                this.TileSet = newInstance;
            }
        }

        /// <inheritdoc/>
        void IAssetComponent<Sprite>.RefreshAsset(Sprite newInstance) {
            this.TileSet?.RefreshSprite(newInstance);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        bool IAssetComponent<AutoTileSet>.TryGetAsset(Guid id, out AutoTileSet asset) {
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

        /// <inheritdoc/>
        bool IAssetComponent<Sprite>.TryGetAsset(Guid id, out Sprite asset) {
            if (this.TileSet != null) {
                this.TileSet.TryGetSprite(id, out asset);
            }
            else {
                asset = null;
            }

            return asset != null;
        }

        /// <inheritdoc/>
        protected override void ClearActiveTiles() {
            this._activeTileToIndex.Clear();
        }

        /// <inheritdoc/>
        protected override Point GetMaximumTile() {
            return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Max(), this._activeTileToIndex.Keys.Select(t => t.Y).Max());
        }

        /// <inheritdoc/>
        protected override Point GetMinimumTile() {
            return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Min(), this._activeTileToIndex.Keys.Select(t => t.Y).Min());
        }

        /// <inheritdoc/>
        protected override bool HasActiveTiles() {
            return this._activeTileToIndex.Any();
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            base.Initialize();

            if (this.TileSet != null) {
                this.TileSet.SpriteChanged += this.TileSet_SpriteChanged;
            }
        }

        /// <inheritdoc/>
        protected override bool TryAddTile(Point tile) {
            var result = false;
            if (!this._activeTileToIndex.ContainsKey(tile)) {
                result = true;

                this._activeTileToIndex[tile] = this.GetIndex(tile);

                for (var x = -1; x <= 1; x++) {
                    for (var y = -1; y <= 1; y++) {
                        this.ReevaluateIndex(tile + new Point(x, y));
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override bool TryRemoveTile(Point tile) {
            return this._activeTileToIndex.Remove(tile);
        }

        private byte GetIndex(Point tile) {
            byte index;
            if (this.TileSet?.UseIntermediateDirections == true) {
                var direction = IntermediateDirections.None;
                if (this.HasActiveTileAt(tile + new Point(0, 1))) {
                    direction |= IntermediateDirections.North;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 0))) {
                    direction |= IntermediateDirections.East;
                }

                if (this.HasActiveTileAt(tile - new Point(0, 1))) {
                    direction |= IntermediateDirections.South;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 0))) {
                    direction |= IntermediateDirections.West;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 1))) {
                    direction |= IntermediateDirections.NorthEast;
                }

                if (this.HasActiveTileAt(tile + new Point(-1, 1))) {
                    direction |= IntermediateDirections.NorthWest;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 1))) {
                    direction |= IntermediateDirections.SouthWest;
                }

                if (this.HasActiveTileAt(tile - new Point(-1, 1))) {
                    direction |= IntermediateDirections.SouthEast;
                }

                index = (byte)direction;
            }
            else {
                var direction = CardinalDirections.None;
                if (this.HasActiveTileAt(tile + new Point(0, 1))) {
                    direction |= CardinalDirections.North;
                }

                if (this.HasActiveTileAt(tile + new Point(1, 0))) {
                    direction |= CardinalDirections.East;
                }

                if (this.HasActiveTileAt(tile - new Point(0, 1))) {
                    direction |= CardinalDirections.South;
                }

                if (this.HasActiveTileAt(tile - new Point(1, 0))) {
                    direction |= CardinalDirections.West;
                }

                index = (byte)direction;
            }

            return index;
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
                this._spriteScales = new Vector2[this.TileSet.Size];

                for (byte i = 0; i < this._spriteScales.Length; i++) {
                    var sprite = this.TileSet.GetSprite(i);
                    this._spriteScales[i] = this.GetTileScale(sprite);
                }
            }
        }

        private void TileSet_SpriteChanged(object sender, byte e) {
            var sprite = this.TileSet.GetSprite(e);
            this._spriteScales[e] = this.GetTileScale(sprite);
        }
    }
}
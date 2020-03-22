namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile map component that is either "on" or "off". The on tiles will show the selected sprite.
    /// </summary>
    public sealed class BinaryTileMap : TileableComponent, IAssetComponent<Sprite>, IDrawableComponent {

        [DataMember]
        private readonly HashSet<Point> _activeTiles = new HashSet<Point>();

        private Sprite _sprite;
        private Vector2 _tileScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTileMap"/> class.
        /// </summary>
        public BinaryTileMap() : base() {
        }

        /// <inheritdoc/>
        public override IReadOnlyCollection<Point> ActiveTiles {
            get {
                return this._activeTiles;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Sprite> AvailableTiles {
            get {
                return new[] { this.Sprite };
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember(Order = 0)]
        public Sprite Sprite {
            get {
                return this._sprite;
            }
            set {
                if (this._sprite != value) {
                    this._sprite = value;
                    this.LoadContent();
                    this._tileScale = this.GetTileScale(this.Sprite);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Sprite?.Texture != null && this._activeTiles.Any()) {
                foreach (var tile in this._activeTiles) {
                    var boundingArea = this.GetTileBoundingArea(tile);
                    if (boundingArea.Overlaps(viewBoundingArea)) {
                        MacabreGame.Instance.SpriteBatch.Draw(this.Sprite, boundingArea.Minimum, this._tileScale, this.Color);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public override bool HasActiveTileAt(Point tilePosition) {
            return this._activeTiles.Contains(tilePosition);
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._sprite?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized) {
                this.Sprite?.Load();
                this._tileScale = this.GetTileScale(this.Sprite);
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        public void RefreshAsset(Sprite newInstance) {
            if (newInstance != null && this.Sprite?.Id == newInstance.Id) {
                this.Sprite = newInstance;
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Sprite = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool SetDefaultTile(Sprite newDefault) {
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out Sprite asset) {
            var result = this.Sprite != null && this.Sprite.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void ClearActiveTiles() {
            this._activeTiles.Clear();
        }

        /// <inheritdoc/>
        protected override Point GetMaximumTile() {
            return new Point(this._activeTiles.Select(t => t.X).Max(), this._activeTiles.Select(t => t.Y).Max());
        }

        /// <inheritdoc/>
        protected override Point GetMinimumTile() {
            return new Point(this._activeTiles.Select(t => t.X).Min(), this._activeTiles.Select(t => t.Y).Min());
        }

        /// <inheritdoc/>
        protected override bool HasActiveTiles() {
            return this._activeTiles.Any();
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            base.Initialize();
            this.TransformChanged += this.Self_TransformChanged;
        }

        /// <inheritdoc/>
        protected override void OnGridChanged() {
            base.OnGridChanged();
            this._tileScale = this.GetTileScale(this.Sprite);
        }

        /// <inheritdoc/>
        protected override bool TryAddTile(Point tile) {
            return this._activeTiles.Add(tile);
        }

        /// <inheritdoc/>
        protected override bool TryRemoveTile(Point tile) {
            return this._activeTiles.Remove(tile);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._tileScale = this.GetTileScale(this.Sprite);
        }
    }
}
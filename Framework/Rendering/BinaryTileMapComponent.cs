namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile map component that is either "on" or "off". The on tiles will show the selected sprite.
    /// </summary>
    public sealed class BinaryTileMapComponent : BaseComponent, IDrawableComponent, IAssetComponent<Sprite>, ITileMap {
        private readonly HashSet<Point> _activeTiles = new HashSet<Point>();
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private Point _mapSize;
        private Sprite _sprite;
        private Point _tileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTileMapComponent"/> class.
        /// </summary>
        public BinaryTileMapComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the size of the map. This is how many grid tiles it will contain.
        /// </summary>
        /// <value>The size of the map.</value>
        public Point MapSize {
            get {
                return this._mapSize;
            }

            set {
                var originalSize = this._mapSize;
                if (this._mapSize != value && value.X > 0 && value.Y > 0) {
                    this._mapSize = value;
                    this._boundingArea.Reset();

                    if (originalSize.X > this._mapSize.X || originalSize.Y > this._mapSize.Y) {
                        this.ResizeActiveTiles();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public Sprite Sprite {
            get {
                return this._sprite;
            }
            set {
                if (this._sprite != value) {
                    this._sprite = value;
                    this.LoadContent();
                    this._boundingArea.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the tiles in pixels. These will still be scaled according to
        /// pixel density and the current object's scale.
        /// </summary>
        /// <value>The size of the tiles.</value>
        [DataMember]
        public Point TileSize {
            get {
                return this._tileSize;
            }

            set {
                if (this._tileSize != value && value.X > 0 && value.Y > 0) {
                    this._tileSize = value;
                    this._boundingArea.Reset();
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, float viewHeight) {
            if (this.Sprite == null || this.Sprite.Texture == null || this._scene?.Game == null) {
                return;
            }

            // TODO: pass in the current camera bounding area to the Draw method and don't render a tile if it isn't within it.
            foreach (var tile in this._activeTiles) {
                var position = new Vector2(tile.X * this.TileSize.X * GameSettings.Instance.InversePixelsPerUnit, tile.Y * this.TileSize.Y * GameSettings.Instance.InversePixelsPerUnit);
                var transform = this.GetWorldTransform(position);
                this._scene.Game.SpriteBatch.Draw(
                    this.Sprite.Texture,
                    transform.Position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(this.Sprite.Location, this.Sprite.Size),
                    this.Color,
                    0f,
                    Vector2.Zero,
                    transform.Scale,
                    SpriteEffects.FlipVertically,
                    0f);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._sprite?.Id == id;
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
        public bool TryGetAsset(Guid id, out Sprite asset) {
            var result = this.Sprite != null && this.Sprite.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;
            var width = this.MapSize.X * this.TileSize.X * inversePixelDensity;
            var height = this.MapSize.X * this.TileSize.X * inversePixelDensity;

            var points = new List<Vector2> {
                this.WorldTransform.Position,
                this.GetWorldTransform(new Vector2(width, 0f)).Position,
                this.GetWorldTransform(new Vector2(width, height)).Position,
                this.GetWorldTransform(new Vector2(0f, height)).Position
            };

            var minimumX = points.Min(x => x.X);
            var minimumY = points.Min(x => x.Y);
            var maximumX = points.Max(x => x.X);
            var maximumY = points.Max(x => x.Y);

            return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }

        private void ResizeActiveTiles() {
            this._activeTiles.RemoveWhere(x => x.X > this.MapSize.X || x.Y > this.MapSize.Y);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
        }
    }
}
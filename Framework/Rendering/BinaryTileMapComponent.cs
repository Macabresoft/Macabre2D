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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Point TileSize {
            get {
                return this.Sprite?.Size ?? Point.Zero;
            }
        }

        /// <summary>
        /// Adds an active tile at the specified position.
        /// </summary>
        /// <param name="tilePosition">The tile position.</param>
        public void AddActiveTile(Point tilePosition) {
            if (this.IsValidTilePosition(tilePosition)) {
                this._activeTiles.Add(tilePosition);
            }
        }

        /// <summary>
        /// Clears all active tiles.
        /// </summary>
        public void ClearActiveTiles() {
            this._activeTiles.Clear();
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, float viewHeight) {
            if (this.Sprite == null || this.Sprite.Texture == null || this._scene?.Game == null) {
                return;
            }

            // TODO: pass in the current camera bounding area to the Draw method and don't render a tile if it isn't within it.
            foreach (var tile in this._activeTiles) {
                var position = new Vector2(tile.X * this.Sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit, tile.Y * this.Sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit);
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

        /// <summary>
        /// Removes the active tile.
        /// </summary>
        /// <param name="tilePosition">The tile position.</param>
        public void RemoveActiveTile(Point tilePosition) {
            this._activeTiles.Remove(tilePosition);
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
            BoundingArea result;
            if (this.Sprite != null) {
                var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;
                var width = this.MapSize.X * this.Sprite.Size.X * inversePixelDensity;
                var height = this.MapSize.X * this.Sprite.Size.X * inversePixelDensity;

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

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private bool IsValidTilePosition(Point position) {
            return position.X >= 0 && position.Y >= 0 && position.X < this.MapSize.X && position.Y < this.MapSize.Y;
        }

        private void ResizeActiveTiles() {
            this._activeTiles.RemoveWhere(x => x.X > this.MapSize.X || x.Y > this.MapSize.Y);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
        }
    }
}
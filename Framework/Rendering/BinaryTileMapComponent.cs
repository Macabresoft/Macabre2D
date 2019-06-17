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
    public sealed class BinaryTileMapComponent : BaseComponent, IDrawableComponent, IAssetComponent<Sprite>, ITileable<Sprite> {
        private readonly HashSet<Point> _activeTiles = new HashSet<Point>();
        private readonly ResettableLazy<BoundingArea> _boundingArea;

        private Point _mapSize;
        private Sprite _sprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTileMapComponent"/> class.
        /// </summary>
        public BinaryTileMapComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc/>
        public IEnumerable<Sprite> AvailableTiles {
            get {
                return new[] { this.Sprite };
            }
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
        public Sprite DefaultTile {
            get {
                return this.Sprite;
            }
        }

        /// <inheritdoc/>
        public Point GridSize {
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

        /// <inheritdoc/>
        [DataMember]
        public Rotation Rotation { get; private set; } = new Rotation();

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

        /// <inheritdoc/>
        public void AddTile(Point position) {
            if (this.IsValidTilePosition(position)) {
                this._activeTiles.Add(position);
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
                var transform = this.GetWorldTransform(position, this.Rotation.Angle);
                this._scene.Game.SpriteBatch.Draw(
                    this.Sprite.Texture,
                    transform.Position * GameSettings.Instance.PixelsPerUnit,
                    new Rectangle(this.Sprite.Location, this.Sprite.Size),
                    this.Color,
                    transform.Rotation.Angle,
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
        public void RemoveTile(Point position) {
            this._activeTiles.Remove(position);
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
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;

            if (this.Rotation == null) {
                this.Rotation = new Rotation();
            }

            this.Rotation.AngleChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Sprite != null) {
                var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;
                var width = this.GridSize.X * this.Sprite.Size.X * inversePixelDensity;
                var height = this.GridSize.Y * this.Sprite.Size.Y * inversePixelDensity;
                var angle = this.Rotation.Angle;

                var points = new List<Vector2> {
                    this.GetWorldTransform(angle).Position,
                    this.GetWorldTransform(new Vector2(width, 0f), angle).Position,
                    this.GetWorldTransform(new Vector2(width, height), angle).Position,
                    this.GetWorldTransform(new Vector2(0f, height), angle).Position
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

        private RotatableTransform CreateRotatableTransform() {
            return this.GetWorldTransform(this.Rotation.Angle);
        }

        private bool IsValidTilePosition(Point position) {
            return position.X >= 0 && position.Y >= 0 && position.X < this.GridSize.X && position.Y < this.GridSize.Y;
        }

        private void ResizeActiveTiles() {
            this._activeTiles.RemoveWhere(x => x.X > this.GridSize.X || x.Y > this.GridSize.Y);
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
        }
    }
}
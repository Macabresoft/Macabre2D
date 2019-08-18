namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tileable component. Contains a <see cref="TileGrid"/> and implements <see cref="ITileable"/>.
    /// </summary>
    public abstract class TileableComponent : BaseComponent, ITileable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly Dictionary<Point, BoundingArea> _tilePositionToBoundingArea = new Dictionary<Point, BoundingArea>();
        private readonly ResettableLazy<TileGrid> _worldGrid;
        private TileGrid _grid = new TileGrid(Vector2.One);
        private Point _maximumTile;
        private Point _minimumTile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileableComponent"/> class.
        /// </summary>
        public TileableComponent() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._worldGrid = new ResettableLazy<TileGrid>(this.CreateWorldGrid);
        }

        /// <inheritdoc/>
        public abstract IReadOnlyCollection<Point> ActiveTiles { get; }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public TileGrid LocalGrid {
            get {
                return this._grid;
            }

            set {
                if (this._grid != value) {
                    this._grid = value;
                    this.OnGridChanged();
                }
            }
        }

        /// <inheritdoc/>
        public TileGrid WorldGrid {
            get {
                return this._worldGrid.Value;
            }
        }

        /// <inheritdoc/>
        public bool AddTile(Point tile) {
            var result = this.TryAddTile(tile);
            if (result) {
                if (tile.X > this._maximumTile.X) {
                    this._maximumTile = new Point(tile.X, this._maximumTile.Y);
                }
                else if (tile.X < this._minimumTile.X) {
                    this._minimumTile = new Point(tile.X, this._minimumTile.Y);
                }

                if (tile.Y > this._maximumTile.Y) {
                    this._maximumTile = new Point(this._maximumTile.X, tile.Y);
                }
                else if (tile.Y < this._minimumTile.Y) {
                    this._minimumTile = new Point(this._minimumTile.X, tile.Y);
                }
            }

            return result;
        }

        /// <summary>
        /// Clears all active tiles.
        /// </summary>
        public void ClearTiles() {
            this.ClearTiles();
            this._minimumTile = Point.Zero;
            this._maximumTile = Point.Zero;
            this.ResetBoundingArea();
        }

        /// <inheritdoc/>
        public Point GetTileThatContains(Vector2 worldPosition) {
            var result = Point.Zero;
            var worldGrid = this.WorldGrid;

            if (worldGrid.TileSize.X > 0 && worldGrid.TileSize.Y > 0) {
                worldPosition -= worldGrid.Offset;
                var xTile = Math.Floor(worldPosition.X / worldGrid.TileSize.X);
                var yTile = Math.Floor(worldPosition.Y / worldGrid.TileSize.Y);
                result = new Point((int)xTile, (int)yTile);
            }

            return result;
        }

        /// <inheritdoc/>
        public abstract bool HasActiveTileAt(Point tilePosition);

        /// <inheritdoc/>
        public bool HasActiveTileAt(Vector2 worldPosition) {
            var tile = this.GetTileThatContains(worldPosition);
            return this.HasActiveTileAt(tile);
        }

        /// <inheritdoc/>
        public bool RemoveTile(Point tile) {
            var result = this.TryRemoveTile(tile);
            if (result) {
                this._tilePositionToBoundingArea.Remove(tile);

                if (tile.X == this._minimumTile.X || tile.Y == this._minimumTile.Y) {
                    this.ResetMinimumTile();
                }

                if (tile.X == this._maximumTile.X || tile.Y == this._maximumTile.Y) {
                    this.ResetMinimumTile();
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the active tiles.
        /// </summary>
        protected abstract void ClearActiveTiles();

        /// <summary>
        /// Gets the maximum tile.
        /// </summary>
        /// <returns>The maximum tile.</returns>
        protected abstract Point GetMaximumTile();

        /// <summary>
        /// Gets the minimum tile.
        /// </summary>
        /// <returns>The minimum tile.</returns>
        protected abstract Point GetMinimumTile();

        /// <summary>
        /// Gets the bounding area for the tile at the specified position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The bounding area.</returns>
        protected BoundingArea GetTileBoundingArea(Point tile) {
            if (!this._tilePositionToBoundingArea.TryGetValue(tile, out var boundingArea)) {
                var worldGrid = this.WorldGrid;
                var tilePosition = worldGrid.GetTilePosition(tile);
                boundingArea = new BoundingArea(tilePosition, tilePosition + worldGrid.TileSize);
                this._tilePositionToBoundingArea.Add(tile, boundingArea);
            }

            return boundingArea;
        }

        /// <summary>
        /// Gets the tile scale for the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <returns></returns>
        protected Vector2 GetTileScale(Sprite sprite) {
            var result = this.WorldGrid.TileSize;
            if (sprite != null && sprite.Size.X != 0 && sprite.Size.Y != 0) {
                var spriteWidth = sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var spriteHeight = sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;
                result = new Vector2(this.WorldGrid.TileSize.X / spriteWidth, this.WorldGrid.TileSize.Y / spriteHeight);
            }

            return result;
        }

        /// <summary>
        /// Determines whether this has active tiles.
        /// </summary>
        /// <returns><c>true</c> if this has active tiles; otherwise, <c>false</c>.</returns>
        protected abstract bool HasActiveTiles();

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
            this.ResetBoundingArea();
            this.ResetMinimumTile();
            this.ResetMaximumTile();
        }

        /// <summary>
        /// Called when <see cref="LocalGrid"/> changes.
        /// </summary>
        protected virtual void OnGridChanged() {
            this._worldGrid.Reset();
            this.ResetBoundingArea();
        }

        /// <summary>
        /// Resets the bounding area.
        /// </summary>
        protected void ResetBoundingArea() {
            this._tilePositionToBoundingArea.Clear();
            this._boundingArea.Reset();
        }

        /// <summary>
        /// Tries to add the specified tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A value indicating whether or not the tile was added.</returns>
        protected abstract bool TryAddTile(Point tile);

        /// <summary>
        /// Tries to remove the specified tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>A value indicating whether or not the tile was removed.</returns>
        protected abstract bool TryRemoveTile(Point tile);

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.HasActiveTiles()) {
                var worldGrid = this.WorldGrid;
                result = new BoundingArea(worldGrid.GetTilePosition(this._minimumTile), worldGrid.GetTilePosition(this._maximumTile + new Point(1, 1)));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private TileGrid CreateWorldGrid() {
            var worldTransform = this.WorldTransform;

            var matrix =
                Matrix.CreateScale(worldTransform.Scale.X, worldTransform.Scale.Y, 1f) *
                Matrix.CreateScale(this.LocalGrid.TileSize.X, this.LocalGrid.TileSize.Y, 1f) *
                Matrix.CreateTranslation(this.LocalGrid.Offset.X, this.LocalGrid.Offset.Y, 0f) *
                Matrix.CreateTranslation(worldTransform.Position.X, worldTransform.Position.Y, 0f);

            var transform = matrix.ToTransform();
            return new TileGrid(transform.Scale, transform.Position);
        }

        private void ResetMaximumTile() {
            if (this.HasActiveTiles()) {
                this._maximumTile = this.GetMaximumTile();
            }
            else {
                this._maximumTile = Point.Zero;
            }
        }

        private void ResetMinimumTile() {
            if (this.HasActiveTiles()) {
                this._minimumTile = this.GetMinimumTile();
            }
            else {
                this._minimumTile = Point.Zero;
            }
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._tilePositionToBoundingArea.Clear();
            this._worldGrid.Reset();
            this.ResetBoundingArea();
        }
    }
}
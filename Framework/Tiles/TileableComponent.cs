namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents four directions from a single tile.
    /// </summary>
    [Flags]
    public enum CardinalDirections : byte {
        None = 0,

        North = 1 << 0,

        West = 1 << 1,

        East = 1 << 2,

        South = 1 << 3,

        All = North | West | East | South
    }

    /// <summary>
    /// A tileable component. Contains a <see cref="TileGrid"/> and implements <see cref="ITileable"/>.
    /// </summary>
    public abstract class TileableComponent : BaseComponent, ITileable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly Dictionary<Point, BoundingArea> _tilePositionToBoundingArea = new Dictionary<Point, BoundingArea>();
        private readonly ResettableLazy<TileGrid> _worldGrid;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileableComponent"/> class.
        /// </summary>
        protected TileableComponent() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._worldGrid = new ResettableLazy<TileGrid>(this.CreateWorldGrid);
            this.GridConfiguration.GridChanged += this.GridConfiguration_GridChanged;
        }

        /// <inheritdoc/>
        public event EventHandler TilesChanged;

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
        public GridConfiguration GridConfiguration { get; private set; } = new GridConfiguration();

        /// <inheritdoc/>
        public Point MaximumTile { get; private set; }

        /// <inheritdoc/>
        public Point MinimumTile { get; private set; }

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
                if (tile.X > this.MaximumTile.X) {
                    this.MaximumTile = new Point(tile.X, this.MaximumTile.Y);
                }
                else if (tile.X < this.MinimumTile.X) {
                    this.MinimumTile = new Point(tile.X, this.MinimumTile.Y);
                }

                if (tile.Y > this.MaximumTile.Y) {
                    this.MaximumTile = new Point(this.MaximumTile.X, tile.Y);
                }
                else if (tile.Y < this.MinimumTile.Y) {
                    this.MinimumTile = new Point(this.MinimumTile.X, tile.Y);
                }

                this.ResetBoundingArea();
                this.TilesChanged.SafeInvoke(this);
            }

            return result;
        }

        /// <summary>
        /// Clears all active tiles.
        /// </summary>
        [ComponentCommand("Clear Tiles")]
        public void ClearTiles() {
            this.ClearActiveTiles();
            this.MinimumTile = Point.Zero;
            this.MaximumTile = Point.Zero;
            this.ResetBoundingArea();
            this.ResetTileBoundingAreas();
            this.TilesChanged.SafeInvoke(this);
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

                if (tile.X == this.MinimumTile.X || tile.Y == this.MinimumTile.Y) {
                    this.ResetMinimumTile();
                    this.ResetBoundingArea();
                }

                if (tile.X == this.MaximumTile.X || tile.Y == this.MaximumTile.Y) {
                    this.ResetMaximumTile();
                    this.ResetBoundingArea();
                }

                this.TilesChanged.SafeInvoke(this);
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
            this._worldGrid.Reset();
            this.ResetBoundingArea();
            this.ResetTileBoundingAreas();
            this.ResetMinimumTile();
            this.ResetMaximumTile();
        }

        /// <summary>
        /// Called when <see cref="LocalGrid"/> changes.
        /// </summary>
        protected virtual void OnGridChanged() {
            this._worldGrid.Reset();
            this.ResetBoundingArea();
            this.ResetTileBoundingAreas();
        }

        /// <summary>
        /// Resets the bounding area.
        /// </summary>
        protected void ResetBoundingArea() {
            this._tilePositionToBoundingArea.Clear();
            this._boundingArea.Reset();
        }

        /// <summary>
        /// Resets the tile bounding areas.
        /// </summary>
        protected void ResetTileBoundingAreas() {
            this._tilePositionToBoundingArea.Clear();
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
                result = new BoundingArea(worldGrid.GetTilePosition(this.MinimumTile), worldGrid.GetTilePosition(this.MaximumTile + new Point(1, 1)));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private TileGrid CreateWorldGrid() {
            var tileGrid = this.GridConfiguration.Grid;
            if (this.GridConfiguration.UseLocalGrid) {
                var worldTransform = this.WorldTransform;

                var matrix =
                    Matrix.CreateScale(worldTransform.Scale.X, worldTransform.Scale.Y, 1f) *
                    Matrix.CreateScale(tileGrid.TileSize.X, tileGrid.TileSize.Y, 1f) *
                    Matrix.CreateTranslation(tileGrid.Offset.X, tileGrid.Offset.Y, 0f) *
                    Matrix.CreateTranslation(worldTransform.Position.X, worldTransform.Position.Y, 0f);

                var transform = matrix.ToTransform();
                tileGrid = new TileGrid(transform.Scale, transform.Position);
            }

            return tileGrid;
        }

        private void GridConfiguration_GridChanged(object sender, EventArgs e) {
            this.OnGridChanged();
        }

        private void ResetMaximumTile() {
            if (this.HasActiveTiles()) {
                this.MaximumTile = this.GetMaximumTile();
            }
            else {
                this.MaximumTile = Point.Zero;
            }

            this._boundingArea.Reset();
        }

        private void ResetMinimumTile() {
            if (this.HasActiveTiles()) {
                this.MinimumTile = this.GetMinimumTile();
            }
            else {
                this.MinimumTile = Point.Zero;
            }

            this._boundingArea.Reset();
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this.OnGridChanged();
        }
    }
}
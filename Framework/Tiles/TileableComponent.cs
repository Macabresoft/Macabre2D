namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tileable component. Contains a <see cref="TileGrid"/> and implements <see cref="ITileable"/>.
    /// </summary>
    public abstract class TileableComponent : BaseComponent, ITileable {

        [DataMember]
        private readonly HashSet<Point> _activeTiles = new HashSet<Point>();

        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly Dictionary<Point, BoundingArea> _tilePositionToBoundingArea = new Dictionary<Point, BoundingArea>();
        private readonly ResettableLazy<Vector2> _tileScale;
        private readonly ResettableLazy<TileGrid> _worldGrid;
        private TileGrid _grid = new TileGrid(Vector2.One);
        private Point _maximumTile;
        private Point _minimumTile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileableComponent"/> class.
        /// </summary>
        public TileableComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._tileScale = new ResettableLazy<Vector2>(this.CreateTileScale);
            this._worldGrid = new ResettableLazy<TileGrid>(this.CreateWorldGrid);
        }

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

        /// <summary>
        /// Gets the active tiles.
        /// </summary>
        /// <value>The active tiles.</value>
        protected IReadOnlyCollection<Point> ActiveTiles {
            get {
                return this._activeTiles;
            }
        }

        /// <summary>
        /// Gets or sets the tile scale.
        /// </summary>
        /// <value>The tile scale.</value>
        protected Vector2 TileScale {
            get {
                return this._tileScale.Value;
            }
        }

        /// <inheritdoc/>
        public bool AddTile(Point tile) {
            var result = this._activeTiles.Add(tile);
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
        public void ClearActiveTiles() {
            this._activeTiles.Clear();
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
        public bool RemoveTile(Point tile) {
            var result = this._activeTiles.Remove(tile);
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
        /// Creates the tile scale.
        /// </summary>
        /// <returns>The tile scale.</returns>
        protected virtual Vector2 CreateTileScale() {
            return this.WorldGrid.TileSize;
        }

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

                // We need to pass TileScale into the draw manually. If we just use BoundingArea
                // here, we can go with the minimum value and bypass the need for transforms at all.
            }

            return boundingArea;
        }

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
            this.ResetTileScale();
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
        /// Resets the tile scale.
        /// </summary>
        protected void ResetTileScale() {
            this._tileScale.Reset();
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this._activeTiles.Any()) {
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
            if (this._activeTiles.Any()) {
                this._maximumTile = new Point(this._activeTiles.Select(t => t.X).Max(), this._activeTiles.Select(t => t.Y).Max());
            }
            else {
                this._maximumTile = Point.Zero;
            }
        }

        private void ResetMinimumTile() {
            if (this._activeTiles.Any()) {
                this._minimumTile = new Point(this._activeTiles.Select(t => t.X).Min(), this._activeTiles.Select(t => t.Y).Min());
            }
            else {
                this._minimumTile = Point.Zero;
            }
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._tilePositionToBoundingArea.Clear();
            this._worldGrid.Reset();
            this.ResetTileScale();
            this.ResetBoundingArea();
        }
    }
}
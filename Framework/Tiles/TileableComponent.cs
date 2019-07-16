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
        private readonly Dictionary<Point, (BoundingArea BoundingArea, Transform Transform)> _tilePositionToBoundingAreaAndTransform = new Dictionary<Point, (BoundingArea, Transform)>();
        private TileGrid _grid = new TileGrid(Vector2.One);
        private Vector2 _maximumPosition;
        private Point _maximumTile;
        private Vector2 _minimumPosition;
        private Point _minimumTile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileableComponent"/> class.
        /// </summary>
        public TileableComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public TileGrid Grid {
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
        protected Vector2 TileScale { get; set; } = Vector2.One;

        /// <inheritdoc/>
        public void AddTile(Point tile) {
            if (this._activeTiles.Add(tile)) {
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
        }

        /// <summary>
        /// Clears all active tiles.
        /// </summary>
        public void ClearActiveTiles() {
            this._activeTiles.Clear();
            this._minimumPosition = Vector2.Zero;
            this._maximumPosition = Vector2.Zero;
            this._minimumTile = Point.Zero;
            this._maximumTile = Point.Zero;
            this.ResetBoundingArea();
        }

        /// <inheritdoc/>
        public Point GetTileThatContains(Vector2 worldPosition) {
            var result = Point.Zero;

            if (this.Grid.TileSize.X > 0 && this.Grid.TileSize.Y > 0) {
                worldPosition -= this.WorldTransform.Position;
                var xTile = Math.Floor(worldPosition.X / this.Grid.TileSize.X);
                var yTile = Math.Floor(worldPosition.Y / this.Grid.TileSize.Y);
                result = new Point((int)xTile, (int)yTile);
            }

            return result;
        }

        /// <inheritdoc/>
        public void RemoveTile(Point tile) {
            if (this._activeTiles.Remove(tile)) {
                this._tilePositionToBoundingAreaAndTransform.Remove(tile);

                if (tile.X == this._maximumTile.X || tile.Y == this._maximumTile.Y || tile.X == this._minimumTile.X || tile.Y == this._minimumTile.Y) {
                    this.ResetPositionValues();
                }
            }
        }

        /// <summary>
        /// Gets the bounding area for the tile at the specified position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The bounding area.</returns>
        protected (BoundingArea BoundingArea, Transform Transform) GetTileBoundingAreaAndTransform(Point tile) {
            if (!this._tilePositionToBoundingAreaAndTransform.TryGetValue(tile, out var boundingAreaAndTransform)) {
                var offset = new Vector2((tile.X * this.Grid.TileSize.X) + this.Grid.Offset.X, (tile.Y * this.Grid.TileSize.Y) + this.Grid.Offset.Y);
                var worldTransform = this.WorldTransform;
                var transform = this.GetWorldTransform(offset, worldTransform.Scale * this.TileScale);
                var boundingArea = new BoundingArea(transform.Position, transform.Position + this.Grid.TileSize * this.LocalScale);
                boundingAreaAndTransform = (boundingArea, transform);
                this._tilePositionToBoundingAreaAndTransform.Add(tile, boundingAreaAndTransform);
            }

            return boundingAreaAndTransform;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;
            this._tilePositionToBoundingAreaAndTransform.Clear();
            this.ResetTileValues();
        }

        /// <summary>
        /// Called when <see cref="Grid"/> changes.
        /// </summary>
        protected virtual void OnGridChanged() {
            this.ResetPositionValues();
        }

        /// <summary>
        /// Resets the bounding area.
        /// </summary>
        protected void ResetBoundingArea() {
            this._tilePositionToBoundingAreaAndTransform.Clear();
            this._boundingArea.Reset();
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this._activeTiles.Any()) {
                var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;
                var width = this._maximumPosition.X - this._minimumPosition.X;
                var height = this._maximumPosition.Y - this._minimumPosition.Y;

                var points = new List<Vector2> {
                    this.GetWorldTransform(this.Grid.Offset).Position,
                    this.GetWorldTransform(new Vector2(width, 0f) + this.Grid.Offset).Position,
                    this.GetWorldTransform(new Vector2(width, height) + this.Grid.Offset).Position,
                    this.GetWorldTransform(new Vector2(0f, height) + this.Grid.Offset).Position
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

        private void ResetPositionValues() {
            this._minimumPosition = new Vector2(this._minimumTile.X * this.Grid.TileSize.X, this._minimumTile.Y * this.Grid.TileSize.Y);
            this._maximumPosition = new Vector2((this._maximumTile.X + 1) * this.Grid.TileSize.X, (this._maximumTile.Y + 1) * this.Grid.TileSize.Y);
            this.ResetBoundingArea();
        }

        private void ResetTileValues() {
            var xValues = this._activeTiles.Select(t => t.X);
            var yValues = this._activeTiles.Select(t => t.Y);
            this._minimumTile = new Point(xValues.Any() ? xValues.Min() : 0, yValues.Any() ? yValues.Min() : 0);
            this._maximumTile = new Point(xValues.Any() ? xValues.Max() : 0, yValues.Any() ? yValues.Max() : 0);
            this._tilePositionToBoundingAreaAndTransform.Clear();
            this.ResetPositionValues();
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._tilePositionToBoundingAreaAndTransform.Clear();
            this.ResetBoundingArea();
        }
    }
}
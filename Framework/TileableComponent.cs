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
        private TileGrid _grid;
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

        /// <inheritdoc/>
        [DataMember]
        public Rotation Rotation { get; private set; } = new Rotation();

        /// <summary>
        /// Gets the active tiles.
        /// </summary>
        /// <value>The active tiles.</value>
        protected IReadOnlyCollection<Point> ActiveTiles {
            get {
                return this._activeTiles;
            }
        }

        /// <inheritdoc/>
        public void AddTile(Point position) {
            this._activeTiles.Add(position);

            if (position.X > this._maximumTile.X) {
                this._maximumTile = new Point(position.X, this._maximumTile.Y);
            }
            else if (position.X < this._minimumTile.X) {
                this._minimumTile = new Point(position.X, this._minimumTile.Y);
            }

            if (position.Y > this._maximumTile.Y) {
                this._maximumTile = new Point(this._maximumTile.X, position.Y);
            }
            else if (position.Y < this._minimumTile.Y) {
                this._minimumTile = new Point(this._minimumTile.X, position.Y);
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
        }

        /// <inheritdoc/>
        public void RemoveTile(Point position) {
            this._activeTiles.Remove(position);

            if (position.X == this._maximumTile.X || position.Y == this._maximumTile.Y || position.X == this._minimumTile.X || position.Y == this._minimumTile.Y) {
                this.ResetPositionValues();
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.TransformChanged += this.Self_TransformChanged;

            if (this.Rotation == null) {
                this.Rotation = new Rotation();
            }

            this.Rotation.AngleChanged += this.Self_TransformChanged;
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
            this._boundingArea.Reset();
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this._activeTiles.Any()) {
                var inversePixelDensity = GameSettings.Instance.InversePixelsPerUnit;
                var width = this._maximumPosition.X - this._minimumPosition.X;
                var height = this._maximumPosition.Y - this._minimumPosition.Y;
                var angle = this.Rotation.Angle;

                var points = new List<Vector2> {
                    this.GetWorldTransform(this.Grid.Offset, angle).Position,
                    this.GetWorldTransform(new Vector2(width, 0f) + this.Grid.Offset, angle).Position,
                    this.GetWorldTransform(new Vector2(width, height) + this.Grid.Offset, angle).Position,
                    this.GetWorldTransform(new Vector2(0f, height) + this.Grid.Offset, angle).Position
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
            this._minimumTile = new Point(xValues.Min(), yValues.Min());
            this._maximumTile = new Point(xValues.Max(), yValues.Max());
            this.ResetPositionValues();
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this.ResetBoundingArea();
        }
    }
}
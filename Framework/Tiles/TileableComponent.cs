namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

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
    /// A tileable component. Contains a <see cref="TileGrid" /> and implements <see
    /// cref="IGameTileableComponent" />.
    /// </summary>
    public abstract class TileableComponent : GameComponent, IGameTileableComponent {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly Dictionary<Point, BoundingArea> _tilePositionToBoundingArea = new Dictionary<Point, BoundingArea>();
        private GridComponent? _gridComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileableComponent" /> class.
        /// </summary>
        protected TileableComponent() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        }

        /// <inheritdoc />
        public event EventHandler? TilesChanged;

        /// <inheritdoc />
        public abstract IReadOnlyCollection<Point> ActiveTiles { get; }

        /// <inheritdoc />
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <inheritdoc />
        public Point MaximumTile { get; private set; }

        /// <inheritdoc />
        public Point MinimumTile { get; private set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Point GetTileThatContains(Vector2 worldPosition) {
            var result = Point.Zero;

            if (this._gridComponent != null) {
                var worldGrid = this._gridComponent.WorldGrid;

                if (worldGrid.TileSize.X > 0 && worldGrid.TileSize.Y > 0) {
                    worldPosition -= worldGrid.Offset;
                    var xTile = Math.Floor(worldPosition.X / worldGrid.TileSize.X);
                    var yTile = Math.Floor(worldPosition.Y / worldGrid.TileSize.Y);
                    result = new Point((int)xTile, (int)yTile);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public abstract bool HasActiveTileAt(Point tilePosition);

        /// <inheritdoc />
        public bool HasActiveTileAt(Vector2 worldPosition) {
            var tile = this.GetTileThatContains(worldPosition);
            return this.HasActiveTileAt(tile);
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            this.SetComponents();
            this.ResetBoundingArea();
            this.ResetTileBoundingAreas();
            this.ResetMinimumTile();
            this.ResetMaximumTile();
        }

        /// <inheritdoc />
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
            if (!this._tilePositionToBoundingArea.TryGetValue(tile, out var boundingArea) && this._gridComponent != null) {
                var worldGrid = this._gridComponent.WorldGrid;
                var tilePosition = worldGrid.GetTilePosition(tile);
                boundingArea = new BoundingArea(tilePosition, tilePosition + worldGrid.TileSize);
                this._tilePositionToBoundingArea.Add(tile, boundingArea);
            }

            return boundingArea;
        }

        /// <summary>
        /// Gets the tile scale for the specified sprite.
        /// </summary>
        /// <param name="spriteSize">The sprite size.</param>
        /// <returns>The scale for the sprite to fit within the tile grid.</returns>
        protected Vector2 GetTileScale(Point spriteSize) {
            var result = Vector2.One;

            if (this._gridComponent != null) {
                var inversePixelsPerUnit = this.Entity.Scene.Game.Project.Settings.InversePixelsPerUnit;
                result = this._gridComponent.WorldGrid.TileSize;
                if (spriteSize.X != 0 && spriteSize.Y != 0) {
                    var spriteWidth = spriteSize.X * inversePixelsPerUnit;
                    var spriteHeight = spriteSize.Y * inversePixelsPerUnit;
                    result = new Vector2(result.X / spriteWidth, result.Y / spriteHeight);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether this has active tiles.
        /// </summary>
        /// <returns><c>true</c> if this has active tiles; otherwise, <c>false</c>.</returns>
        protected abstract bool HasActiveTiles();

        /// <summary>
        /// Resets the bounding area.
        /// </summary>
        protected void ResetBoundingArea() {
            this._tilePositionToBoundingArea.Clear();
            this._boundingArea.Reset();
        }

        /// <summary>
        /// Called when <see cref="LocalGrid" /> changes.
        /// </summary>
        protected virtual void ResetBoundingAreas() {
            this.ResetBoundingArea();
            this.ResetTileBoundingAreas();
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
            if (this._gridComponent != null && this.HasActiveTiles()) {
                var worldGrid = this._gridComponent.WorldGrid;
                result = new BoundingArea(worldGrid.GetTilePosition(this.MinimumTile), worldGrid.GetTilePosition(this.MaximumTile + new Point(1, 1)));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private void GridComponent_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(GridComponent.WorldGrid)) {
                this.ResetBoundingAreas();
            }
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

        private void SetComponents() {
            if (this._gridComponent != null) {
                this._gridComponent.PropertyChanged += this.GridComponent_PropertyChanged;
            }

            if (this.Entity.TryGetAncestralComponent(out this._gridComponent) && this._gridComponent != null) {
                this._gridComponent.PropertyChanged += this.GridComponent_PropertyChanged;
            }
        }
    }
}
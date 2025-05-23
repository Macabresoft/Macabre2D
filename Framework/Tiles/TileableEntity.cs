namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A tileable entity.
/// <see cref="ITileableEntity" />
/// </summary>
public abstract class TileableEntity : Entity, ITileableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly Dictionary<Point, BoundingArea> _tilePositionToBoundingArea = new();

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? TilesChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileableEntity" /> class.
    /// </summary>
    protected TileableEntity() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public abstract IReadOnlyCollection<Point> ActiveTiles { get; }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    public IGridContainer CurrentGrid { get; private set; } = GridContainer.Empty;

    /// <inheritdoc />
    public Point MaximumTile { get; private set; }

    /// <inheritdoc />
    public Point MinimumTile { get; private set; }

    /// <inheritdoc />
    public bool AddTile(Point tile) {
        var isFirst = !this.HasActiveTiles();
        var result = this.TryAddTile(tile);
        if (result) {
            if (isFirst) {
                this.MaximumTile = tile;
                this.MinimumTile = tile;
            }
            else {
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
            }

            this.ResetBoundingArea();
            this.TilesChanged.SafeInvoke(this);
        }

        return result;
    }

    /// <summary>
    /// Clears all active tiles.
    /// </summary>
    [EntityCommand("Clear Tiles")]
    public void ClearTiles() {
        this.ClearActiveTiles();
        this.MinimumTile = Point.Zero;
        this.MaximumTile = Point.Zero;
        this.ResetBoundingArea();
        this.TilesChanged.SafeInvoke(this);
    }

    /// <inheritdoc />
    public BoundingArea GetTileBoundingArea(Point tile) {
        if (!this._tilePositionToBoundingArea.TryGetValue(tile, out var boundingArea)) {
            var grid = this.CurrentGrid;
            var tilePosition = grid.GetTilePosition(tile) + this.LocalPosition;
            boundingArea = new BoundingArea(tilePosition, tilePosition + grid.TileSize);
            this._tilePositionToBoundingArea.Add(tile, boundingArea);
        }

        return boundingArea;
    }

    /// <inheritdoc />
    public Point GetTileThatContains(Vector2 worldPosition) {
        var result = Point.Zero;
        var grid = this.CurrentGrid;

        if (grid.TileSize is { X: > 0, Y: > 0 }) {
            var relativePosition = worldPosition - grid.WorldPosition;
            var xTile = Math.Floor(relativePosition.X / grid.TileSize.X);
            var yTile = Math.Floor(relativePosition.Y / grid.TileSize.Y);
            result = new Point((int)xTile, (int)yTile);
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
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ResetGridContainer();
        this.ResetMinimumTile();
        this.ResetMaximumTile();
    }

    /// <inheritdoc />
    public override void OnRemovedFromSceneTree() {
        this.CurrentGrid.PropertyChanged -= this.GridContainer_PropertyChanged;
        base.OnRemovedFromSceneTree();
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
    protected virtual Point GetMaximumTile() {
        return new Point(this.ActiveTiles.Select(t => t.X).Max(), this.ActiveTiles.Select(t => t.Y).Max());
    }

    /// <summary>
    /// Gets the minimum tile.
    /// </summary>
    /// <returns>The minimum tile.</returns>
    protected virtual Point GetMinimumTile() {
        return new Point(this.ActiveTiles.Select(t => t.X).Min(), this.ActiveTiles.Select(t => t.Y).Min());
    }

    /// <summary>
    /// Gets the tile scale for the specified sprite.
    /// </summary>
    /// <param name="spriteSize">The sprite size.</param>
    /// <returns>The scale for the sprite to fit within the tile grid.</returns>
    protected Vector2 GetTileScale(Point spriteSize) {
        var inversePixelsPerUnit = this.Project.UnitsPerPixel;
        var result = this.CurrentGrid.TileSize;

        if (spriteSize.X != 0 && spriteSize.Y != 0) {
            var spriteWidth = spriteSize.X * inversePixelsPerUnit;
            var spriteHeight = spriteSize.Y * inversePixelsPerUnit;
            result = new Vector2(result.X / spriteWidth, result.Y / spriteHeight);
        }

        return result;
    }

    /// <summary>
    /// Determines whether this has active tiles.
    /// </summary>
    /// <returns><c>true</c> if this has active tiles; otherwise, <c>false</c>.</returns>
    protected virtual bool HasActiveTiles() => this.ActiveTiles.Any();

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.Parent)) {
            this.ResetGridContainer();
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.ResetBoundingArea();
    }

    /// <summary>
    /// Resets the bounding area.
    /// </summary>
    protected virtual void ResetBoundingArea() {
        this._tilePositionToBoundingArea.Clear();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
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
        if (this.HasActiveTiles() && this.CurrentGrid is { } grid && grid != GridContainer.Empty) {
            result = new BoundingArea(grid.GetTilePosition(this.MinimumTile) + this.LocalPosition, grid.GetTilePosition(this.MaximumTile + new Point(1, 1)) + this.LocalPosition);
        }
        else {
            result = new BoundingArea();
        }

        return result;
    }

    private void GridContainer_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IGridContainer.TileSize)) {
            this.ResetBoundingArea();
            this.TilesChanged?.SafeInvoke(this);
        }
    }

    private void ResetGridContainer() {
        this.CurrentGrid.PropertyChanged -= this.GridContainer_PropertyChanged;

        if (this.TryGetAncestor<IGridContainer>(out var gridContainer)) {
            this.CurrentGrid = gridContainer;
            this.CurrentGrid.PropertyChanged += this.GridContainer_PropertyChanged;
        }
        else {
            this.CurrentGrid = GridContainer.Empty;
        }

        this.ResetBoundingArea();
        this.TilesChanged.SafeInvoke(this);
    }

    private void ResetMaximumTile() {
        this.MaximumTile = this.HasActiveTiles() ? this.GetMaximumTile() : Point.Zero;
        this._boundingArea.Reset();
    }

    private void ResetMinimumTile() {
        this.MinimumTile = this.HasActiveTiles() ? this.GetMinimumTile() : Point.Zero;
        this._boundingArea.Reset();
    }
}
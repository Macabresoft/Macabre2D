namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

/// <summary>
/// Defines the visual behavior for a tile map.
/// </summary>
public enum AutoTileMapVisualBehavior {
    /// <summary>
    /// Visuals are determined by the surrounding tiles. The map can create non-box shapes.
    /// </summary>
    Standard,

    /// <summary>
    /// Visuals always create single unit width columns.
    /// </summary>
    Column,

    /// <summary>
    /// Visuals always create single unit height rows.
    /// </summary>
    Row,

    /// <summary>
    /// Creates visual chunks by scanning the columns for grouped together tiles. Only forms box shapes.
    /// </summary>
    [Display(Name = "Box (Column Preferred)")]
    BoxColumn,

    /// <summary>
    /// Creates visual chunks by scanning the rows for grouped together tiles. Only forms box shapes.
    /// </summary>
    [Display(Name = "Box (Row Preferred)")]
    BoxRow
}

/// <summary>
/// An entity which maps an <see cref="AutoTileSet" /> onto a grid.
/// </summary>
[Display(Name = "Auto Tile Map")]
[Category(CommonCategories.TileMap)]
public sealed class AutoTileMap : RenderableTileMap {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<Point, byte> _activeTileToIndex = new();

    private Color _color = Color.White;
    private Vector2 _previousWorldScale;
    private Vector2 _spriteScale = Vector2.One;
    private AutoTileMapVisualBehavior _visualBehavior;

    /// <inheritdoc />
    public override IReadOnlyCollection<Point> ActiveTiles => this._activeTileToIndex.Keys;

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets the animation reference.
    /// </summary>
    [DataMember(Order = 0, Name = "Tile Set")]
    public AutoTileSetReference TileSetReference { get; } = new();

    [DataMember]
    public AutoTileMapVisualBehavior VisualBehavior {
        get => this._visualBehavior;
        set {
            if (this.Set(ref this._visualBehavior, value) && this.IsInitialized) {
                this.ReevaluateIndexes();
            }
        }
    }

    /// <inheritdoc />
    public override bool HasActiveTileAt(Point tilePosition) {
        return this._activeTileToIndex.ContainsKey(tilePosition);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.TileSetReference.PropertyChanged -= this.TileSetReference_PropertyChanged;
        this.TileSetReference.Initialize(this.Scene.Assets);
        this.TileSetReference.PropertyChanged += this.TileSetReference_PropertyChanged;
        this._previousWorldScale = this.Transform.Scale;
        this.ReevaluateIndexes();
        this.ResetSpriteScale();
    }


    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch && this.TileSetReference.PackagedAsset is { } tileSet && this.TileSetReference.Asset is { } spriteSheet) {
            foreach (var (activeTile, tileIndex) in this._activeTileToIndex) {
                var boundingArea = this.GetTileBoundingArea(activeTile);
                if (boundingArea.Overlaps(viewBoundingArea) && tileSet.TryGetSpriteIndex(tileIndex, out var spriteIndex) && (this.Scene.BoundingArea.IsEmpty || boundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                    spriteBatch.Draw(
                        this.Settings.PixelsPerUnit,
                        spriteSheet,
                        spriteIndex,
                        boundingArea.Minimum,
                        this._spriteScale,
                        this.Color);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        this._activeTileToIndex.Clear();
    }

    /// <inheritdoc />
    protected override Point GetMaximumTile() {
        return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Max(), this._activeTileToIndex.Keys.Select(t => t.Y).Max());
    }

    /// <inheritdoc />
    protected override Point GetMinimumTile() {
        return new Point(this._activeTileToIndex.Keys.Select(t => t.X).Min(), this._activeTileToIndex.Keys.Select(t => t.Y).Min());
    }

    /// <inheritdoc />
    protected override bool HasActiveTiles() {
        return this._activeTileToIndex.Any();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.Transform) && this.Transform.Scale != this._previousWorldScale) {
            this._previousWorldScale = this.Transform.Scale;
            this.ResetSpriteScale();
        }
    }

    /// <inheritdoc />
    protected override void ResetBoundingArea() {
        base.ResetBoundingArea();
        this.ResetSpriteScale();
    }

    /// <inheritdoc />
    protected override bool TryAddTile(Point tile) {
        var result = false;
        if (!this._activeTileToIndex.ContainsKey(tile)) {
            result = true;

            this._activeTileToIndex[tile] = this.GetIndex(tile);
            this.ReevaluateSurroundingIndexes(tile);
        }

        return result;
    }

    /// <inheritdoc />
    protected override bool TryRemoveTile(Point tile) {
        var result = this._activeTileToIndex.Remove(tile);
        if (result) {
            this.ReevaluateSurroundingIndexes(tile);
        }

        return result;
    }

    private byte GetColumnIndex(Point tile) {
        var direction = CardinalDirections.None;
        if (this.HasActiveTileAt(tile + new Point(0, 1))) {
            direction |= CardinalDirections.North;
        }

        if (this.HasActiveTileAt(tile - new Point(0, 1))) {
            direction |= CardinalDirections.South;
        }

        return (byte)direction;
    }

    private byte GetIndex(Point tile) {
        return this.VisualBehavior switch {
            AutoTileMapVisualBehavior.Standard => this.GetStandardIndex(tile),
            AutoTileMapVisualBehavior.Column => this.GetColumnIndex(tile),
            AutoTileMapVisualBehavior.Row => this.GetRowIndex(tile),
            AutoTileMapVisualBehavior.BoxColumn => 0,
            AutoTileMapVisualBehavior.BoxRow => 0,
            _ => 0
        };
    }

    private byte GetRowIndex(Point tile) {
        var direction = CardinalDirections.None;

        if (this.HasActiveTileAt(tile + new Point(1, 0))) {
            direction |= CardinalDirections.East;
        }

        if (this.HasActiveTileAt(tile - new Point(1, 0))) {
            direction |= CardinalDirections.West;
        }

        return (byte)direction;
    }

    private byte GetStandardIndex(Point tile) {
        var direction = CardinalDirections.None;
        if (this.HasActiveTileAt(tile + new Point(0, 1))) {
            direction |= CardinalDirections.North;
        }

        if (this.HasActiveTileAt(tile + new Point(1, 0))) {
            direction |= CardinalDirections.East;
        }

        if (this.HasActiveTileAt(tile - new Point(0, 1))) {
            direction |= CardinalDirections.South;
        }

        if (this.HasActiveTileAt(tile - new Point(1, 0))) {
            direction |= CardinalDirections.West;
        }

        return (byte)direction;
    }

    private void ReevaluateBoxColumnIndexes() {
        if (this.HasActiveTiles()) {
            var visualChunks = new List<VisualChunk>();
            var orderedTiles = this.ActiveTiles.OrderBy(tile => tile.X).ThenBy(tile => tile.Y).ToList();
            var currentVisualChunk = new VisualChunk(orderedTiles.First());
            visualChunks.Add(currentVisualChunk);
            orderedTiles.RemoveAt(0);

            foreach (var activeTile in orderedTiles.Where(activeTile => !currentVisualChunk.TryAddToColumn(activeTile))) {
                currentVisualChunk = new VisualChunk(activeTile);
                visualChunks.Add(currentVisualChunk);
            }

            var reversedVisualChunks = visualChunks.OrderByDescending(x => x.MaximumTile.X).ToList();
            foreach (var visualChunk in reversedVisualChunks) {
                var candidates = visualChunks.Where(x => x.MaximumTile.X == visualChunk.MinimumTile.X - 1).ToList();

                if (candidates.Any(candidate => candidate.TryCombineColumn(visualChunk))) {
                    visualChunks.Remove(visualChunk);
                }
            }

            foreach (var visualChunk in visualChunks) {
                this.SetIndexesForVisualChunk(visualChunk);
            }
        }
    }

    private void ReevaluateBoxRowIndexes() {
        if (this.HasActiveTiles()) {
            var visualChunks = new List<VisualChunk>();
            var orderedTiles = this.ActiveTiles.OrderBy(tile => tile.Y).ThenBy(tile => tile.X).ToList();
            var currentVisualChunk = new VisualChunk(orderedTiles.First());
            visualChunks.Add(currentVisualChunk);
            orderedTiles.RemoveAt(0);

            foreach (var activeTile in orderedTiles.Where(activeTile => !currentVisualChunk.TryAddToRow(activeTile))) {
                currentVisualChunk = new VisualChunk(activeTile);
                visualChunks.Add(currentVisualChunk);
            }

            var reversedVisualChunks = visualChunks.OrderByDescending(x => x.MaximumTile.Y).ToList();
            foreach (var visualChunk in reversedVisualChunks) {
                var candidates = visualChunks.Where(x => x.MaximumTile.Y == visualChunk.MinimumTile.Y - 1).ToList();

                if (candidates.Any(candidate => candidate.TryCombineRow(visualChunk))) {
                    visualChunks.Remove(visualChunk);
                }
            }

            foreach (var visualChunk in visualChunks) {
                this.SetIndexesForVisualChunk(visualChunk);
            }
        }
    }

    private void ReevaluateIndex(Point tile) {
        if (this.HasActiveTileAt(tile)) {
            this._activeTileToIndex[tile] = this.GetIndex(tile);
        }
    }

    private void ReevaluateIndexes() {
        switch (this.VisualBehavior) {
            case AutoTileMapVisualBehavior.Standard:
            case AutoTileMapVisualBehavior.Column:
            case AutoTileMapVisualBehavior.Row:
                this.ReevaluateIndexesStandard();
                break;
            case AutoTileMapVisualBehavior.BoxColumn:
                this.ReevaluateBoxColumnIndexes();
                break;
            case AutoTileMapVisualBehavior.BoxRow:
                this.ReevaluateBoxRowIndexes();
                break;
        }
    }

    private void ReevaluateIndexesStandard() {
        var clonedActiveTiles = this._activeTileToIndex.Keys.ToList();
        foreach (var activeTile in clonedActiveTiles) {
            this.ReevaluateIndex(activeTile);
        }
    }

    private void ReevaluateSurroundingIndexes(Point tile) {
        if (this.VisualBehavior is AutoTileMapVisualBehavior.BoxColumn or AutoTileMapVisualBehavior.BoxRow) {
            this.ReevaluateIndexes();
        }
        else {
            for (var x = -1; x <= 1; x++) {
                for (var y = -1; y <= 1; y++) {
                    this.ReevaluateIndex(tile + new Point(x, y));
                }
            }
        }
    }

    private void ResetSpriteScale() {
        if (this.TileSetReference.Asset is { } spriteSheet) {
            this._spriteScale = this.GetTileScale(spriteSheet.SpriteSize);
        }
    }

    private void SetIndexesForVisualChunk(VisualChunk visualChunk) {
        if (visualChunk.MaximumTile == visualChunk.MinimumTile) {
            this._activeTileToIndex[visualChunk.MaximumTile] = 0;
        }
        else if (visualChunk.MaximumTile.X == visualChunk.MinimumTile.X) {
            this._activeTileToIndex[visualChunk.MinimumTile] = (byte)CardinalDirections.North;
            this._activeTileToIndex[visualChunk.MaximumTile] = (byte)CardinalDirections.South;

            for (var y = visualChunk.MinimumTile.Y + 1; y < visualChunk.MaximumTile.Y; y++) {
                this._activeTileToIndex[new Point(visualChunk.MinimumTile.X, y)] = (byte)(CardinalDirections.South | CardinalDirections.North);
            }
        }
        else if (visualChunk.MaximumTile.Y == visualChunk.MinimumTile.Y) {
            this._activeTileToIndex[visualChunk.MinimumTile] = (byte)CardinalDirections.East;
            this._activeTileToIndex[visualChunk.MaximumTile] = (byte)CardinalDirections.West;

            for (var x = visualChunk.MinimumTile.X + 1; x < visualChunk.MaximumTile.X; x++) {
                this._activeTileToIndex[new Point(x, visualChunk.MinimumTile.Y)] = (byte)(CardinalDirections.East | CardinalDirections.West);
            }
        }
        else {
            for (var x = visualChunk.MinimumTile.X; x <= visualChunk.MaximumTile.X; x++) {
                for (var y = visualChunk.MinimumTile.Y; y <= visualChunk.MaximumTile.Y; y++) {
                    var current = new Point(x, y);
                    if (current.X == visualChunk.MinimumTile.X) {
                        if (current.Y == visualChunk.MinimumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.NorthEast;
                        }
                        else if (current.Y == visualChunk.MaximumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.SouthEast;
                        }
                        else {
                            this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthEast | CardinalDirections.South);
                        }
                    }
                    else if (current.X == visualChunk.MaximumTile.X) {
                        if (current.Y == visualChunk.MinimumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.NorthWest;
                        }
                        else if (current.Y == visualChunk.MaximumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.SouthWest;
                        }
                        else {
                            this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthWest | CardinalDirections.South);
                        }
                    }
                    else if (current.Y == visualChunk.MinimumTile.Y) {
                        this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthWest | CardinalDirections.East);
                    }
                    else if (current.Y == visualChunk.MaximumTile.Y) {
                        this._activeTileToIndex[current] = (byte)(CardinalDirections.SouthWest | CardinalDirections.East);
                    }
                    else {
                        this._activeTileToIndex[current] = (byte)CardinalDirections.All;
                    }
                }
            }
        }
    }

    private void TileSetReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(AutoTileSetReference.Asset) or nameof(AutoTileSetReference.PackagedAsset) or nameof(SpriteSheetAsset.SpriteSize)) {
            this.ResetBoundingArea();
        }
    }

    private class VisualChunk {
        public VisualChunk(Point tile) {
            this.MinimumTile = tile;
            this.MaximumTile = tile;
        }

        public VisualChunk(Point minimumTile, Point maximumTile) {
            this.MinimumTile = minimumTile;
            this.MaximumTile = maximumTile;
        }

        public Point MaximumTile { get; set; }

        public Point MinimumTile { get; set; }

        public bool TryAddToColumn(Point tile) {
            if (this.MaximumTile.X == this.MinimumTile.X) {
                if (tile.Y == this.MinimumTile.Y - 1) {
                    this.MinimumTile = tile;
                    return true;
                }

                if (tile.Y == this.MaximumTile.Y + 1) {
                    this.MaximumTile = tile;
                    return true;
                }
            }

            return false;
        }

        public bool TryAddToRow(Point tile) {
            if (this.MaximumTile.Y == this.MinimumTile.Y) {
                if (tile.X == this.MinimumTile.X - 1) {
                    this.MinimumTile = tile;
                    return true;
                }

                if (tile.X == this.MaximumTile.X + 1) {
                    this.MaximumTile = tile;
                    return true;
                }
            }

            return false;
        }

        public bool TryCombineColumn(VisualChunk other) {
            if (this.MinimumTile.Y == other.MinimumTile.Y && this.MaximumTile.Y == other.MaximumTile.Y) {
                if (this.MaximumTile.X == other.MinimumTile.X - 1 || this.MinimumTile.X + 1 == other.MaximumTile.X) {
                    this.MaximumTile = new Point(Math.Max(this.MaximumTile.X, other.MaximumTile.X), this.MaximumTile.Y);
                    this.MinimumTile = new Point(Math.Min(this.MinimumTile.X, other.MinimumTile.X), this.MinimumTile.Y);
                    return true;
                }
            }

            return false;
        }

        public bool TryCombineRow(VisualChunk other) {
            if (this.MinimumTile.X == other.MinimumTile.X && this.MaximumTile.X == other.MaximumTile.X) {
                if (this.MaximumTile.Y == other.MinimumTile.Y - 1 || this.MinimumTile.Y + 1 == other.MaximumTile.Y) {
                    this.MaximumTile = new Point(this.MaximumTile.X, Math.Max(this.MaximumTile.Y, other.MaximumTile.Y));
                    this.MinimumTile = new Point(this.MinimumTile.X, Math.Min(this.MinimumTile.Y, other.MinimumTile.Y));
                    return true;
                }
            }

            return false;
        }
    }
}
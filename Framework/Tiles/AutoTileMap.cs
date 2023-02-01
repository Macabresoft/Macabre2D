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
    SingleColumn,

    /// <summary>
    /// Visuals always create single unit height rows.
    /// </summary>
    SingleRow,

    /// <summary>
    /// Creates visual chunks by scanning the columns for grouped together tiles.
    /// </summary>
    Column,

    /// <summary>
    /// Creates visual chunks by scanning the rows for grouped together tiles.
    /// </summary>
    Row
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
    private Vector2 _spriteScale = Vector2.One;
    private AutoTileMapVisualBehavior _visualBehavior;

    /// <inheritdoc />
    public override IReadOnlyCollection<Point> ActiveTiles => this._activeTileToIndex.Keys;

    /// <summary>
    /// Gets the animation reference.
    /// </summary>
    [DataMember(Order = 0, Name = "Tile Set")]
    public AutoTileSetReference TileSetReference { get; } = new();

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    [DataMember]
    public AutoTileMapVisualBehavior VisualBehavior {
        get => this._visualBehavior;
        set {
            if (this.Set(ref this._visualBehavior, value)) {
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
        this.ReevaluateIndexes();
        this.ResetSpriteScale();
    }


    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch && this.TileSetReference is { PackagedAsset: { } tileSet, Asset: { } spriteSheet }) {
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
            AutoTileMapVisualBehavior.SingleColumn => this.GetColumnIndex(tile),
            AutoTileMapVisualBehavior.SingleRow => this.GetRowIndex(tile),
            AutoTileMapVisualBehavior.Column => 0,
            AutoTileMapVisualBehavior.Row => 0,
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
        var visualChunks = TileChunk.GetColumnChunks(this.ActiveTiles);

        foreach (var visualChunk in visualChunks) {
            this.SetIndexesForVisualChunk(visualChunk);
        }
    }

    private void ReevaluateBoxRowIndexes() {
        var visualChunks = TileChunk.GetRowChunks(this.ActiveTiles);

        foreach (var visualChunk in visualChunks) {
            this.SetIndexesForVisualChunk(visualChunk);
        }
    }

    private void ReevaluateIndex(Point tile) {
        if (this.HasActiveTileAt(tile)) {
            this._activeTileToIndex[tile] = this.GetIndex(tile);
        }
    }

    private void ReevaluateIndexes() {
        if (this.IsInitialized) {
            switch (this.VisualBehavior) {
                case AutoTileMapVisualBehavior.Standard:
                case AutoTileMapVisualBehavior.SingleColumn:
                case AutoTileMapVisualBehavior.SingleRow:
                    this.ReevaluateIndexesStandard();
                    break;
                case AutoTileMapVisualBehavior.Column:
                    this.ReevaluateBoxColumnIndexes();
                    break;
                case AutoTileMapVisualBehavior.Row:
                    this.ReevaluateBoxRowIndexes();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ReevaluateIndexesStandard() {
        var clonedActiveTiles = this._activeTileToIndex.Keys.ToList();
        foreach (var activeTile in clonedActiveTiles) {
            this.ReevaluateIndex(activeTile);
        }
    }

    private void ReevaluateSurroundingIndexes(Point tile) {
        if (this.VisualBehavior is AutoTileMapVisualBehavior.Column or AutoTileMapVisualBehavior.Row) {
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

    private void SetIndexesForVisualChunk(TileChunk tileChunk) {
        if (tileChunk.MaximumTile == tileChunk.MinimumTile) {
            this._activeTileToIndex[tileChunk.MaximumTile] = 0;
        }
        else if (tileChunk.MaximumTile.X == tileChunk.MinimumTile.X) {
            this._activeTileToIndex[tileChunk.MinimumTile] = (byte)CardinalDirections.North;
            this._activeTileToIndex[tileChunk.MaximumTile] = (byte)CardinalDirections.South;

            for (var y = tileChunk.MinimumTile.Y + 1; y < tileChunk.MaximumTile.Y; y++) {
                this._activeTileToIndex[new Point(tileChunk.MinimumTile.X, y)] = (byte)(CardinalDirections.South | CardinalDirections.North);
            }
        }
        else if (tileChunk.MaximumTile.Y == tileChunk.MinimumTile.Y) {
            this._activeTileToIndex[tileChunk.MinimumTile] = (byte)CardinalDirections.East;
            this._activeTileToIndex[tileChunk.MaximumTile] = (byte)CardinalDirections.West;

            for (var x = tileChunk.MinimumTile.X + 1; x < tileChunk.MaximumTile.X; x++) {
                this._activeTileToIndex[new Point(x, tileChunk.MinimumTile.Y)] = (byte)(CardinalDirections.East | CardinalDirections.West);
            }
        }
        else {
            for (var x = tileChunk.MinimumTile.X; x <= tileChunk.MaximumTile.X; x++) {
                for (var y = tileChunk.MinimumTile.Y; y <= tileChunk.MaximumTile.Y; y++) {
                    var current = new Point(x, y);
                    if (current.X == tileChunk.MinimumTile.X) {
                        if (current.Y == tileChunk.MinimumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.NorthEast;
                        }
                        else if (current.Y == tileChunk.MaximumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.SouthEast;
                        }
                        else {
                            this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthEast | CardinalDirections.South);
                        }
                    }
                    else if (current.X == tileChunk.MaximumTile.X) {
                        if (current.Y == tileChunk.MinimumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.NorthWest;
                        }
                        else if (current.Y == tileChunk.MaximumTile.Y) {
                            this._activeTileToIndex[current] = (byte)CardinalDirections.SouthWest;
                        }
                        else {
                            this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthWest | CardinalDirections.South);
                        }
                    }
                    else if (current.Y == tileChunk.MinimumTile.Y) {
                        this._activeTileToIndex[current] = (byte)(CardinalDirections.NorthWest | CardinalDirections.East);
                    }
                    else if (current.Y == tileChunk.MaximumTile.Y) {
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
}
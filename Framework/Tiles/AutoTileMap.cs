namespace Macabresoft.Macabre2D.Framework;

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
        // TODO: Create visual chunks
    }

    private void ReevaluateBoxRowIndexes() {
        // TODO: Create visual chunks
    }

    private void ReevaluateIndex(Point tile) {
        if (this.HasActiveTileAt(tile)) {
            this._activeTileToIndex[tile] = this.GetIndex(tile);
        }
    }

    private void ReevaluateIndexes() {
        // TODO: clear visual chunks

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
        for (var x = -1; x <= 1; x++) {
            for (var y = -1; y <= 1; y++) {
                this.ReevaluateIndex(tile + new Point(x, y));
            }
        }
    }

    private void ResetSpriteScale() {
        if (this.TileSetReference.Asset is { } spriteSheet) {
            this._spriteScale = this.GetTileScale(spriteSheet.SpriteSize);
        }
    }

    private void TileSetReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(AutoTileSetReference.Asset) or nameof(AutoTileSetReference.PackagedAsset) or nameof(SpriteSheetAsset.SpriteSize)) {
            this.ResetBoundingArea();
        }
    }

    /*private struct VisualChunk {
        public VisualChunk(Point minimumTile, Point maximumTile) {
            this.MinimumTile = minimumTile;
            this.MaximumTile = maximumTile;
        }

        public Point MinimumTile { get; }

        public Point MaximumTile { get; }
    }*/
}
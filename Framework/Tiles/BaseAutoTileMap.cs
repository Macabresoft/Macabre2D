namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

/// <summary>
/// A base class for an entity which maps <see cref="AutoTileSet" /> onto a grid.
/// </summary>
[Category(CommonCategories.TileMap)]
public abstract class BaseAutoTileMap : RenderableTileMap {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<Point, byte> _activeTileToIndex = new();

    private Color _color = Color.White;
    private Vector2 _previousWorldScale;
    private Vector2 _spriteScale = Vector2.One;

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

    /// <summary>
    /// Gets the sprite index for a particular tile.
    /// </summary>
    /// <param name="tile">The tile.</param>
    /// <returns>The sprite index.</returns>
    protected abstract byte GetIndex(Point tile);

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

    private void ReevaluateIndex(Point tile) {
        if (this._activeTileToIndex.ContainsKey(tile)) {
            this._activeTileToIndex[tile] = this.GetIndex(tile);
        }
    }

    private void ReevaluateIndexes() {
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
}
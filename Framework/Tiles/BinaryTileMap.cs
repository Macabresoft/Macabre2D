namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A tile map that is either "on" or "off". The on tiles will show the selected sprite.
/// </summary>
[Category(CommonCategories.TileMap)]
public sealed class BinaryTileMap : RenderableTileMap {
    [DataMember]
    private readonly HashSet<Point> _activeTiles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryTileMap" /> class.
    /// </summary>
    public BinaryTileMap() : base() {
        this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<Point> ActiveTiles => this._activeTiles;

    /// <summary>
    /// Gets the sprite reference.
    /// </summary>
    [DataMember(Order = 0)]
    public SpriteReference SpriteReference { get; } = new();

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <inheritdoc />
    public override bool HasActiveTileAt(Point tilePosition) => this._activeTiles.Contains(tilePosition);

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ReorderActiveTiles();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteBatch is { } spriteBatch && this.SpriteReference.Asset is { } spriteSheet && this.OrderedActiveTiles.Any()) {
            var tileBoundingAreas = this.OrderedActiveTiles
                .Select(this.GetTileBoundingArea)
                .Where(boundingArea => boundingArea.Overlaps(viewBoundingArea));

            if (!this.Scene.BoundingArea.IsEmpty) {
                tileBoundingAreas = tileBoundingAreas.Where(boundingArea => boundingArea.OverlapsExclusive(this.Scene.BoundingArea));
            }

            foreach (var boundingArea in tileBoundingAreas) {
                spriteBatch.Draw(
                    this.Project.PixelsPerUnit,
                    spriteSheet,
                    this.SpriteReference.SpriteIndex,
                    boundingArea.Minimum,
                    colorOverride);
            }
        }
    }

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        base.ClearActiveTiles();
        this._activeTiles.Clear();
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SpriteReference;
    }
    
    /// <inheritdoc />
    protected override bool HasActiveTiles() => this._activeTiles.Any();

    /// <inheritdoc />
    protected override bool TryAddTile(Point tile) {
        if (this._activeTiles.Add(tile)) {
            this.ReorderActiveTiles();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool TryRemoveTile(Point tile) {
        if (this._activeTiles.Remove(tile)) {
            this.ReorderActiveTiles();
            return true;
        }

        return false;
    }

    private void SpriteReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(SpriteSheet.SpriteSize)) {
            this.ResetBoundingArea();
        }
    }
}
namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A tile map that presents a prefab at each active tile.
/// </summary>
public class PrefabTileMap : TileableEntity, IRenderableEntity {
    [DataMember]
    private readonly HashSet<Point> _activeTiles = new();

    private readonly Dictionary<Point, IEntity> _activeTileToEntity = new();
    private bool _isVisible;

    /// <inheritdoc />
    public override IReadOnlyCollection<Point> ActiveTiles => this._activeTiles;

    /// <inheritdoc />
    public PixelSnap PixelSnap { get; } = PixelSnap.No;

    /// <summary>
    /// Gets a reference to the prefab this entity contains.
    /// </summary>
    [DataMember(Order = 0, Name = "Prefab")]
    public PrefabReference PrefabReference { get; } = new();

    /// <inheritdoc />
    public bool IsVisible {
        get => this._isVisible && this.IsEnabled && BaseGame.IsDesignMode;
        set => this.Set(ref this._isVisible, value);
    }

    /// <inheritdoc />
    public bool RenderOutOfBounds { get; set; } = true;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.PrefabReference.PropertyChanged -= this.PrefabReference_PropertyChanged;
        this.DeinitializePrefabs();
    }

    /// <inheritdoc />
    public override bool HasActiveTileAt(Point tilePosition) => this._activeTiles.Contains(tilePosition);

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.IsVisible = BaseGame.IsDesignMode;
        this.Reset();
        this.PrefabReference.PropertyChanged += this.PrefabReference_PropertyChanged;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, Color.White);
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (BaseGame.IsDesignMode && this._activeTileToEntity.Any()) {
            var renderables = this._activeTileToEntity.Values.SelectMany(x => x.GetDescendants<IRenderableEntity>().OrderBy(x => x.RenderOrder));

            foreach (var renderable in renderables) {
                renderable.Render(frameTime, viewBoundingArea);
            }
        }
    }

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        this.DeinitializePrefabs();
        this._activeTiles.Clear();
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.PrefabReference;
    }

    /// <inheritdoc />
    protected override bool TryAddTile(Point tile) {
        var result = false;
        if (!this._activeTileToEntity.ContainsKey(tile)) {
            this._activeTiles.Add(tile);
            this.FillTile(tile);
            result = true;
        }

        return result;
    }

    /// <inheritdoc />
    protected override bool TryRemoveTile(Point tile) {
        var result = this.RemovePrefab(tile);
        this._activeTiles.Remove(tile);
        return result;
    }

    private void DeinitializePrefabs() {
        foreach (var entity in this._activeTileToEntity.Values) {
            this.RemovePrefabFromChildren(entity);
        }

        this._activeTileToEntity.Clear();
    }

    private void FillTile(Point tile) {
        this.RemovePrefab(tile);

        if (this.PrefabReference.Asset?.Content is { } entity && entity.TryClone(out var clone)) {
            clone.SetWorldPosition(this.GetTileBoundingArea(tile).Minimum);
            this._activeTileToEntity[tile] = clone;

            if (BaseGame.IsDesignMode) {
                clone.Initialize(this.Scene, this);
                this.ResetBoundingArea();
            }
            else {
                this.AddChild(clone);
            }
        }
    }

    private void PrefabReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.PrefabReference.ContentId)) {
            this.Reset();
        }
    }

    private bool RemovePrefab(Point tile) {
        if (this._activeTileToEntity.Remove(tile, out var entity)) {
            this.RemovePrefabFromChildren(entity);
            return true;
        }

        return false;
    }

    private void RemovePrefabFromChildren(IEntity entity) {
        entity.Deinitialize();
        this.RemoveChild(entity);
    }

    private void Reset() {
        this.DeinitializePrefabs();

        if (this.PrefabReference.Asset?.Content != null) {
            foreach (var tile in this._activeTiles) {
                this.FillTile(tile);
            }
        }
    }
}
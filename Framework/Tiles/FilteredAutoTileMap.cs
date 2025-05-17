namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Defines the way in which this filter can consume its parent auto tile map.
/// </summary>
public enum TileFilter {
    None,
    Edges,
    TopEdge,
    BottomEdge,
    LeftEdge,
    RightEdge,
    Middle,
    All
}

/// <summary>
/// An entity which will filter an <see cref="AutoTileMap" />. Requires a <see cref="AutoTileMap" /> as a parent.
/// </summary>
public class FilteredAutoTileMap : Entity, IRenderableEntity, IActiveTileableEntity {
    private readonly ResettableLazy<IReadOnlyDictionary<Point, byte>> _activeTileToIndex;
    private TileFilter _filter;
    private bool _shouldRender = true;
    private ITileableEntity _tileEntity = EmptyObject.Instance;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredAutoTileMap" /> class.
    /// </summary>
    public FilteredAutoTileMap() : base() {
        this._activeTileToIndex = new ResettableLazy<IReadOnlyDictionary<Point, byte>>(this.GetOrderedActiveTilesToIndex);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._tileEntity.BoundingArea;

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    [DataMember]
    public TileFilter Filter {
        get => this._filter;
        set {
            if (this._filter != value) {
                this._filter = value;
                this._activeTileToIndex.Reset();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    [PredefinedInteger(PredefinedIntegerKind.RenderOrder)]
    public int RenderOrder { get; set; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.TileSetReference.Asset?.DefaultRenderPriority ?? default;
        }

        set {
            this.RenderPriorityOverride.IsEnabled = true;
            this.RenderPriorityOverride.Value = value;
        }
    }

    /// <summary>
    /// Gets a render priority override.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriorityOverride RenderPriorityOverride { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public bool ShouldRender {
        get => this._shouldRender && this.IsEnabled;
        set {
            if (this.Set(ref this._shouldRender, value)) {
                this.ShouldRenderChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets the animation reference.
    /// </summary>
    [DataMember(Order = 0, Name = "Tile Set")]
    public AutoTileSetReference TileSetReference { get; } = new();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._tileEntity.TilesChanged -= this.AutoTileMap_TilesChanged;
        this._tileEntity.BoundingAreaChanged -= this.TileEntityBoundingAreaChanged;
        this._tileEntity = EmptyObject.Instance;
    }

    /// <inheritdoc />
    public bool HasActiveTileAt(Point tilePosition) => this._tileEntity.HasActiveTileAt(tilePosition);

    /// <inheritdoc />
    public bool HasActiveTileAt(Vector2 worldPosition) => this._tileEntity.HasActiveTileAt(worldPosition);

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._tileEntity = this.Parent as ITileableEntity ?? EmptyObject.Instance;
        this._tileEntity.TilesChanged += this.AutoTileMap_TilesChanged;
        this._tileEntity.BoundingAreaChanged += this.TileEntityBoundingAreaChanged;
        this._activeTileToIndex.Reset();
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteBatch is { } spriteBatch && this.TileSetReference is { PackagedAsset: { } tileSet, Asset: { } spriteSheet }) {
            var activeTileToIndex = this._activeTileToIndex.Value;
            foreach (var (activeTile, tileIndex) in activeTileToIndex) {
                var boundingArea = this._tileEntity.GetTileBoundingArea(activeTile);
                if (boundingArea.Overlaps(viewBoundingArea) && tileSet.TryGetSpriteIndex(tileIndex, out var spriteIndex) && (this.Scene.BoundingArea.IsEmpty || boundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                    spriteBatch.Draw(
                        this.Project.PixelsPerUnit,
                        spriteSheet,
                        spriteIndex,
                        boundingArea.Minimum,
                        colorOverride);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.TileSetReference;
    }

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this._shouldRender) {
            this.ShouldRenderChanged.SafeInvoke(this);
        }
    }

    private void AutoTileMap_TilesChanged(object? sender, EventArgs e) {
        this._activeTileToIndex.Reset();
    }

    private IReadOnlyDictionary<Point, byte> GetOrderedActiveTilesToIndex() {
        var result = new Dictionary<Point, byte>();

        if (this.Filter == TileFilter.None || !this._tileEntity.ActiveTiles.Any()) {
            return result;
        }

        if (this.Filter == TileFilter.LeftEdge) {
            foreach (var activeTile in this._tileEntity.ActiveTiles.Where(tile => !this._tileEntity.HasActiveTileAt(new Point(tile.X - 1, tile.Y)))) {
                result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
            }
        }
        else if (this.Filter == TileFilter.TopEdge) {
            foreach (var activeTile in this._tileEntity.ActiveTiles.Where(tile => !this._tileEntity.HasActiveTileAt(new Point(tile.X, tile.Y + 1)))) {
                result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
            }
        }
        else if (this.Filter == TileFilter.RightEdge) {
            foreach (var activeTile in this._tileEntity.ActiveTiles.Where(tile => !this._tileEntity.HasActiveTileAt(new Point(tile.X + 1, tile.Y)))) {
                result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
            }
        }
        else if (this.Filter == TileFilter.BottomEdge) {
            foreach (var activeTile in this._tileEntity.ActiveTiles.Where(tile => !this._tileEntity.HasActiveTileAt(new Point(tile.X, tile.Y - 1)))) {
                result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
            }
        }
        else if (this.Filter == TileFilter.Edges) {
            foreach (var activeTile in this._tileEntity.ActiveTiles) {
                if (!this._tileEntity.HasActiveTileAt(new Point(activeTile.X - 1, activeTile.Y)) ||
                    !this._tileEntity.HasActiveTileAt(new Point(activeTile.X, activeTile.Y + 1)) ||
                    !this._tileEntity.HasActiveTileAt(new Point(activeTile.X + 1, activeTile.Y)) ||
                    !this._tileEntity.HasActiveTileAt(new Point(activeTile.X, activeTile.Y - 1))) {
                    result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
                }
            }
        }
        else if (this.Filter == TileFilter.Middle) {
            foreach (var activeTile in this._tileEntity.ActiveTiles) {
                if (this._tileEntity.HasActiveTileAt(new Point(activeTile.X - 1, activeTile.Y)) &&
                    this._tileEntity.HasActiveTileAt(new Point(activeTile.X, activeTile.Y + 1)) &&
                    this._tileEntity.HasActiveTileAt(new Point(activeTile.X + 1, activeTile.Y)) &&
                    this._tileEntity.HasActiveTileAt(new Point(activeTile.X, activeTile.Y - 1))) {
                    result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
                }
            }
        }
        else if (this.Filter == TileFilter.All) {
            foreach (var activeTile in this._tileEntity.ActiveTiles) {
                result[activeTile] = this._tileEntity.GetStandardAutoTileIndex(activeTile);
            }
        }

        return result;
    }

    private void TileEntityBoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}
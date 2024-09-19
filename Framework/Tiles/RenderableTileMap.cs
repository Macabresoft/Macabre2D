namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// When rendering a tile map, specifies where the render should start. This is important when sprites may overlap in the map.
/// </summary>
public enum TileMapRenderStartingPoint {
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

/// <summary>
/// A base renderable tile map.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableTileMap : TileableEntity, IRenderableEntity {
    private readonly List<Point> _orderedActiveTiles = new();
    private int _renderOrder;
    private TileMapRenderStartingPoint _renderStartingPoint = TileMapRenderStartingPoint.BottomLeft;
    private bool _shouldRender = true;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public PixelSnap PixelSnap { get; set; } = PixelSnap.Inherit;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    [PredefinedInteger(PredefinedIntegerKind.RenderOrder)]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <summary>
    /// Gets or sets the render starting point.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public TileMapRenderStartingPoint RenderStartingPoint {
        get => this._renderStartingPoint;
        set {
            if (this._renderStartingPoint != value) {
                this._renderStartingPoint = value;

                if (this.IsInitialized) {
                    this.ReorderActiveTiles();
                }
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public bool ShouldRender {
        get => this._shouldRender && this.IsEnabled;
        set => this.Set(ref this._shouldRender, value);
    }

    /// <summary>
    /// Gets the active tiles in an order defined by <see cref="RenderStartingPoint" />.
    /// </summary>
    protected IReadOnlyCollection<Point> OrderedActiveTiles => this._orderedActiveTiles;

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        this._orderedActiveTiles.Clear();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.IsEnabled) && this._shouldRender) {
            this.RaisePropertyChanged(nameof(this.ShouldRender));
        }
    }

    /// <summary>
    /// Reorders the active tiles.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Missed a switch case ordering active tiles.</exception>
    protected virtual void ReorderActiveTiles() {
        var orderedPoints = this.ActiveTiles.ToList();
        switch (this.RenderStartingPoint) {
            case TileMapRenderStartingPoint.BottomLeft:
                orderedPoints.Sort(this.CompareTilesBottomLeft);
                break;
            case TileMapRenderStartingPoint.BottomRight:
                orderedPoints.Sort(this.CompareTilesBottomRight);
                break;
            case TileMapRenderStartingPoint.TopLeft:
                orderedPoints.Sort(this.CompareTilesTopLeft);
                break;
            case TileMapRenderStartingPoint.TopRight:
                orderedPoints.Sort(this.CompareTilesTopRight);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Missed a switch case ordering active tiles: {this.RenderStartingPoint}");
        }

        this._orderedActiveTiles.Clear();
        this._orderedActiveTiles.AddRange(orderedPoints);
    }

    private int CompareTilesBottomLeft(Point primary, Point secondary) {
        if (primary.Y < secondary.Y) {
            return 1;
        }

        if (secondary.Y < primary.Y) {
            return -1;
        }

        if (primary.X < secondary.X) {
            return 1;
        }

        if (secondary.X < primary.X) {
            return -1;
        }

        return 0;
    }

    private int CompareTilesBottomRight(Point primary, Point secondary) {
        if (primary.Y < secondary.Y) {
            return 1;
        }

        if (secondary.Y < primary.Y) {
            return -1;
        }

        if (primary.X > secondary.X) {
            return 1;
        }

        if (secondary.X > primary.X) {
            return -1;
        }

        return 0;
    }

    private int CompareTilesTopLeft(Point primary, Point secondary) {
        if (primary.Y > secondary.Y) {
            return 1;
        }

        if (secondary.Y > primary.Y) {
            return -1;
        }

        if (primary.X < secondary.X) {
            return 1;
        }

        if (secondary.X < primary.X) {
            return -1;
        }

        return 0;
    }

    private int CompareTilesTopRight(Point primary, Point secondary) {
        if (primary.Y > secondary.Y) {
            return 1;
        }

        if (secondary.Y > primary.Y) {
            return -1;
        }

        if (primary.X > secondary.X) {
            return 1;
        }

        if (secondary.X > primary.X) {
            return -1;
        }

        return 0;
    }
}
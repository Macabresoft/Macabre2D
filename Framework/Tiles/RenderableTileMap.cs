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
public abstract class RenderableTileMap : RenderableTileableEntity, IRenderableEntity {
    private readonly List<Point> _orderedActiveTiles = [];

    /// <inheritdoc />
    public override RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.SpriteSheet?.DefaultRenderPriority ?? default;
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

    /// <summary>
    /// Gets or sets the render starting point.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public TileMapRenderStartingPoint RenderStartingPoint {
        get;
        set {
            if (field != value) {
                field = value;

                if (this.IsInitialized) {
                    this.ReorderActiveTiles();
                }
            }
        }
    } = TileMapRenderStartingPoint.BottomLeft;


    /// <summary>
    /// Gets the active tiles in an order defined by <see cref="RenderStartingPoint" />.
    /// </summary>
    protected IReadOnlyCollection<Point> OrderedActiveTiles => this._orderedActiveTiles;

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheet? SpriteSheet { get; }

    /// <inheritdoc />
    protected override void ClearActiveTiles() {
        this._orderedActiveTiles.Clear();
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
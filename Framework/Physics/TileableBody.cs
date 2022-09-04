namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents diagonal directions for slops.
/// </summary>
[Flags]
public enum DiagonalDirections : byte {
    [Display(Name = null)]
    None = 0,

    [Display(Name = "North East")]
    NorthEast = 1 << 0,

    [Display(Name = "North West")]
    NorthWest = 1 << 1,

    [Display(Name = "South East")]
    SouthEast = 1 << 2,

    [Display(Name = "South West")]
    SouthWest = 1 << 3
}

/// <summary>
/// A <see cref="PhysicsBody" /> which reacts to a <see cref="ITileableEntity" /> parent
/// and creates colliders based on the available grid.
/// </summary>
[Display(Name = "Tileable Body")]
public sealed class TileableBody : PhysicsBody {
    private readonly List<Collider> _colliders = new();

    private Layers _overrideLayersBottomEdge = Layers.None;
    private Layers _overrideLayersLeftEdge = Layers.None;
    private Layers _overrideLayersRightEdge = Layers.None;
    private Layers _overrideLayersTopEdge = Layers.None;

    private Padding _padding = new();
    private DiagonalDirections _slopedCorners = DiagonalDirections.None;
    private bool _slopeWhenSingleUnitTall = true;
    private ITileableEntity? _tileable;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._tileable?.BoundingArea ?? new BoundingArea();

    /// <inheritdoc />
    public override bool HasCollider => this._colliders.Any();


    /// <summary>
    /// Gets the bottom edge's overriden layer;
    /// </summary>
    [DataMember(Name = "Bottom Layers", Order = 103)]
    public Layers OverrideLayersBottomEdge {
        get => this._overrideLayersBottomEdge;
        set => this.Set(ref this._overrideLayersBottomEdge, value);
    }

    /// <summary>
    /// Gets the left edge's overriden layer;
    /// </summary>
    [DataMember(Name = "Left Layers", Order = 100)]
    public Layers OverrideLayersLeftEdge {
        get => this._overrideLayersLeftEdge;
        set => this.Set(ref this._overrideLayersLeftEdge, value);
    }

    /// <summary>
    /// Gets the right edge's overriden layer;
    /// </summary>
    [DataMember(Name = "Right Layers", Order = 102)]
    public Layers OverrideLayersRightEdge {
        get => this._overrideLayersRightEdge;
        set => this.Set(ref this._overrideLayersRightEdge, value);
    }

    /// <summary>
    /// Gets the top edge's overriden layer;
    /// </summary>
    [DataMember(Name = "Top Layers", Order = 101)]
    public Layers OverrideLayersTopEdge {
        get => this._overrideLayersTopEdge;
        set => this.Set(ref this._overrideLayersTopEdge, value);
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    [DataMember]
    public Padding Padding {
        get => this._padding;
        set {
            value = new Padding(
                Math.Clamp(value.Left, 0f, 1f),
                Math.Clamp(value.Top, 0f, 1f),
                Math.Clamp(value.Right, 0f, 1f),
                Math.Clamp(value.Bottom, 0f, 1f));

            if (this.Set(ref this._padding, value)) {
                this.ResetColliders();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating which corners are sloped.
    /// </summary>
    [DataMember(Name = "Sloped Corners")]
    public DiagonalDirections SlopedCorners {
        get => this._slopedCorners;
        set {
            if (value != this._slopedCorners) {
                this._slopedCorners = value;
                this.ResetColliders();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that determines whether or not this should allow single unit slopes.
    /// </summary>
    [DataMember]
    public bool SlopeWhenSingleUnitTall {
        get => this._slopeWhenSingleUnitTall;
        set {
            if (value != this._slopeWhenSingleUnitTall) {
                this._slopeWhenSingleUnitTall = value;
                this.ResetColliders();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        return this._colliders;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._tileable != null) {
            this._tileable.TilesChanged -= this.OnRequestReset;
        }

        if (this.TryGetParentEntity(out this._tileable) && this._tileable != null) {
            this._tileable.TilesChanged += this.OnRequestReset;
        }
        else {
            this._tileable = null;
        }

        this.ResetColliders();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(ITransformable.Transform)) {
            this.OnRequestReset(sender, e);
        }
    }

    private CardinalDirections GetEdgeDirections(Point tile) {
        var directions = CardinalDirections.None;

        if (this._tileable != null) {
            if (!this._tileable.HasActiveTileAt(tile - new Point(1, 0))) {
                directions |= CardinalDirections.West;
            }

            if (!this._tileable.HasActiveTileAt(tile + new Point(0, 1))) {
                directions |= CardinalDirections.North;
            }

            if (!this._tileable.HasActiveTileAt(tile + new Point(1, 0))) {
                directions |= CardinalDirections.East;
            }

            if (!this._tileable.HasActiveTileAt(tile - new Point(0, 1))) {
                directions |= CardinalDirections.South;
            }
        }

        return directions;
    }

    private void OnRequestReset(object? sender, EventArgs e) {
        this.ResetColliders();
    }

    private void ResetColliders() {
        this._colliders.Clear();

        if (this._tileable != null) {
            var allSegments = new List<TileLineSegment>();

            for (var y = this._tileable.MinimumTile.Y; y <= this._tileable.MaximumTile.Y; y++) {
                for (var x = this._tileable.MinimumTile.X; x <= this._tileable.MaximumTile.X; x++) {
                    var currentTile = new Point(x, y);
                    if (this._tileable.HasActiveTileAt(currentTile)) {
                        var directions = this.GetEdgeDirections(currentTile);
                        var handledDirections = CardinalDirections.None;
                        if (this.SlopedCorners != DiagonalDirections.None) {
                            switch (directions) {
                                case CardinalDirections.NorthWest or CardinalDirections.NorthWest | CardinalDirections.South:
                                    if (this.SlopedCorners.HasFlag(DiagonalDirections.NorthWest) && (this.SlopeWhenSingleUnitTall || !directions.HasFlag(CardinalDirections.South))) {
                                        handledDirections = CardinalDirections.NorthWest;
                                        allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(1, 1), this._overrideLayersTopEdge, handledDirections));
                                    }

                                    break;
                                case CardinalDirections.NorthEast or CardinalDirections.NorthEast | CardinalDirections.South:
                                    if (this.SlopedCorners.HasFlag(DiagonalDirections.NorthEast) && (this.SlopeWhenSingleUnitTall || !directions.HasFlag(CardinalDirections.South))) {
                                        handledDirections = CardinalDirections.NorthEast;
                                        allSegments.Add(new TileLineSegment(currentTile + new Point(0, 1), currentTile + new Point(1, 0), this._overrideLayersTopEdge, handledDirections));
                                    }

                                    break;
                                case CardinalDirections.SouthWest:
                                    if (this.SlopedCorners.HasFlag(DiagonalDirections.SouthWest)) {
                                        handledDirections = CardinalDirections.SouthWest;
                                        allSegments.Add(new TileLineSegment(currentTile + new Point(0, 1), currentTile + new Point(1, 0), this._overrideLayersBottomEdge, handledDirections));
                                    }

                                    break;
                                case CardinalDirections.SouthEast:
                                    if (this.SlopedCorners.HasFlag(DiagonalDirections.SouthEast)) {
                                        handledDirections = CardinalDirections.SouthEast;
                                        allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(1, 1), this._overrideLayersBottomEdge, handledDirections));
                                    }

                                    break;
                            }
                        }

                        if (directions.HasFlag(CardinalDirections.West) && !handledDirections.HasFlag(CardinalDirections.West)) {
                            allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(0, 1), this._overrideLayersLeftEdge, directions));
                        }

                        if (directions.HasFlag(CardinalDirections.North) && !handledDirections.HasFlag(CardinalDirections.North)) {
                            allSegments.Add(new TileLineSegment(currentTile + new Point(0, 1), currentTile + new Point(1, 1), this._overrideLayersTopEdge, directions));
                        }

                        if (directions.HasFlag(CardinalDirections.East) && !handledDirections.HasFlag(CardinalDirections.East)) {
                            allSegments.Add(new TileLineSegment(currentTile + new Point(1, 0), currentTile + new Point(1, 1), this._overrideLayersRightEdge, directions));
                        }

                        if (directions.HasFlag(CardinalDirections.South) && !handledDirections.HasFlag(CardinalDirections.South)) {
                            allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(1, 0), this._overrideLayersBottomEdge, directions));
                        }
                    }
                }
            }

            var removedSegments = new List<TileLineSegment>();
            var horizontalSegments = allSegments.Where(x => x.IsHorizontal).OrderBy(x => x.StartPoint.X).ToList();
            foreach (var segment in horizontalSegments) {
                if (!removedSegments.Contains(segment)) {
                    var compatibleSegments = horizontalSegments.Except(removedSegments).Where(x => x.StartPoint.Y == segment.StartPoint.Y).OrderBy(x => x.StartPoint.X).ToList();
                    removedSegments.AddRange(compatibleSegments.Where(compatibleSegment => segment.TryCombineWith(compatibleSegment)));
                }
            }

            var verticalSegments = allSegments.Where(x => !x.IsVertical).OrderBy(x => x.StartPoint.Y).ToList();
            foreach (var segment in verticalSegments) {
                if (!removedSegments.Contains(segment)) {
                    var compatibleSegments = verticalSegments.Except(removedSegments).Where(x => x.StartPoint.X == segment.StartPoint.X).OrderBy(x => x.StartPoint.Y).ToList();
                    removedSegments.AddRange(compatibleSegments.Where(compatibleSegment => segment.TryCombineWith(compatibleSegment)));
                }
            }

            var diagonalSegments = allSegments.Where(x => !x.IsVertical && !x.IsHorizontal).OrderBy(x => x.StartPoint.X).ToList();
            foreach (var segment in diagonalSegments) {
                if (!removedSegments.Contains(segment)) {
                    var compatibleSegments = diagonalSegments.Except(removedSegments).Where(x => x.StartPoint == segment.EndPoint).OrderBy(x => x.StartPoint.X).ToList();
                    removedSegments.AddRange(compatibleSegments.Where(compatibleSegment => segment.TryCombineWith(compatibleSegment)));
                }
            }

            foreach (var removedSegment in removedSegments) {
                allSegments.Remove(removedSegment);
            }

            foreach (var segment in allSegments) {
                var start = this._tileable.CurrentGrid.GetTilePosition(segment.StartPoint);
                var end = this._tileable.CurrentGrid.GetTilePosition(segment.EndPoint);
                var tileSize = this._tileable.CurrentGrid.LocalTileSize;

                if (segment.EdgeDirections.HasFlag(CardinalDirections.West) && this.Padding.Left > 0f) {
                    start = new Vector2(start.X + this.Padding.Left * tileSize.X, start.Y);
                    end = new Vector2(end.X + this.Padding.Left * tileSize.X, end.Y);
                }
                else if (segment.EdgeDirections.HasFlag(CardinalDirections.East) && this.Padding.Right > 0f) {
                    start = new Vector2(start.X - this.Padding.Right * tileSize.X, start.Y);
                    end = new Vector2(end.X - this.Padding.Right * tileSize.X, end.Y);
                }

                if (segment.EdgeDirections.HasFlag(CardinalDirections.North) && this.Padding.Top > 0f) {
                    start = new Vector2(start.X, start.Y - this.Padding.Top * tileSize.Y);
                    end = new Vector2(end.X, end.Y - this.Padding.Top * tileSize.Y);
                }
                else if (segment.EdgeDirections.HasFlag(CardinalDirections.South) && this.Padding.Bottom > 0f) {
                    start = new Vector2(start.X, start.Y + this.Padding.Bottom * tileSize.Y);
                    end = new Vector2(end.X, end.Y + this.Padding.Bottom * tileSize.Y);
                }

                var collider = new LineCollider(start, end) {
                    Layers = segment.Layers
                };

                collider.Initialize(this);
                this._colliders.Add(collider);
            }
        }
    }

    private class TileLineSegment {
        public TileLineSegment(Point firstPoint, Point secondPoint, Layers layers, CardinalDirections edgeDirections) {
            this.Layers = layers;
            this.EdgeDirections = edgeDirections;

            if (firstPoint.X == secondPoint.X) {
                this.StartPoint = new Point(firstPoint.X, Math.Min(firstPoint.Y, secondPoint.Y));
                this.EndPoint = new Point(firstPoint.X, Math.Max(firstPoint.Y, secondPoint.Y));
            }
            else if (firstPoint.Y == secondPoint.Y) {
                this.StartPoint = new Point(Math.Min(firstPoint.X, secondPoint.X), firstPoint.Y);
                this.EndPoint = new Point(Math.Max(firstPoint.X, secondPoint.X), firstPoint.Y);
            }
            else {
                this.StartPoint = firstPoint;
                this.EndPoint = secondPoint;
            }
        }

        public CardinalDirections EdgeDirections { get; }

        public bool IsHorizontal => this.StartPoint.Y == this.EndPoint.Y;

        public bool IsVertical => this.StartPoint.X == this.EndPoint.X;

        public Layers Layers { get; }

        public Point EndPoint { get; private set; }

        public Point StartPoint { get; private set; }

        public bool TryCombineWith(TileLineSegment otherLine) {
            var result = false;
            if (this.IsVertical && otherLine.IsVertical) {
                if (this.StartPoint.Y == otherLine.EndPoint.Y || this.EndPoint.Y == otherLine.StartPoint.Y) {
                    this.StartPoint = new Point(this.StartPoint.X, Math.Min(this.StartPoint.Y, otherLine.StartPoint.Y));
                    this.EndPoint = new Point(this.StartPoint.X, Math.Max(this.EndPoint.Y, otherLine.EndPoint.Y));
                    result = true;
                }
            }
            else if (this.IsHorizontal && otherLine.IsHorizontal) {
                if (this.StartPoint.X == otherLine.EndPoint.X || this.EndPoint.X == otherLine.StartPoint.X) {
                    this.StartPoint = new Point(Math.Min(this.StartPoint.X, otherLine.StartPoint.X), this.StartPoint.Y);
                    this.EndPoint = new Point(Math.Max(this.EndPoint.X, otherLine.EndPoint.X), this.StartPoint.Y);
                    result = true;
                }
            }
            else if (!this.IsHorizontal && !this.IsVertical && !otherLine.IsHorizontal && !otherLine.IsVertical && this.EndPoint == otherLine.StartPoint) {
                var thisDirection = this.EndPoint - this.StartPoint;
                var otherDirection = otherLine.EndPoint - otherLine.StartPoint;

                if (thisDirection == otherDirection) {
                    this.EndPoint = otherLine.EndPoint;
                }
            }

            return result;
        }
    }
}
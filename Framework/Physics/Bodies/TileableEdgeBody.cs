namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="PhysicsBody" /> which reacts to a <see cref="ITileableEntity" /> parent
/// and creates colliders based on the available grid.
/// </summary>
public class TileableEdgeBody : QuadBody {
    private readonly List<Collider> _colliders = [];
    private ITileableEntity? _tileable;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._tileable?.BoundingArea ?? new BoundingArea();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        foreach (var collider in this._colliders) {
            collider.Deinitialize();
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() => this._colliders;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._tileable != null) {
            this._tileable.BoundingAreaChanged -= this.Tileable_BoundingAreaChanged;
            this._tileable.TilesChanged -= this.OnRequestReset;
        }

        if (this.TryGetAncestor(out this._tileable) && this._tileable != null) {
            this._tileable.BoundingAreaChanged += this.Tileable_BoundingAreaChanged;
            this._tileable.TilesChanged += this.OnRequestReset;
        }
        else {
            this._tileable = null;
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.ResetColliders();
    }

    /// <inheritdoc />
    protected override void ResetColliders() {
        this._colliders.Clear();

        if (this._tileable != null && this._tileable.ActiveTiles.Any()) {
            var allSegments = new List<TileLineSegment>();

            for (var y = this._tileable.MinimumTile.Y; y <= this._tileable.MaximumTile.Y; y++) {
                for (var x = this._tileable.MinimumTile.X; x <= this._tileable.MaximumTile.X; x++) {
                    var currentTile = new Point(x, y);
                    if (this._tileable.HasActiveTileAt(currentTile)) {
                        var directions = this.GetEdgeDirections(currentTile);

                        if (directions.HasFlag(CardinalDirections.West) &&
                            (!this.OverrideLayersLeftEdge.IsEnabled || this.OverrideLayersLeftEdge.Value != Layers.None)) {
                            allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(0, 1), this.OverrideLayersLeftEdge.Value));
                        }

                        if (directions.HasFlag(CardinalDirections.North) &&
                            (!this.OverrideLayersTopEdge.IsEnabled || this.OverrideLayersTopEdge.Value != Layers.None)) {
                            allSegments.Add(new TileLineSegment(currentTile + new Point(0, 1), currentTile + new Point(1, 1), this.OverrideLayersTopEdge.Value));
                        }

                        if (directions.HasFlag(CardinalDirections.East) &&
                            (!this.OverrideLayersRightEdge.IsEnabled || this.OverrideLayersRightEdge.Value != Layers.None)) {
                            allSegments.Add(new TileLineSegment(currentTile + new Point(1, 0), currentTile + new Point(1, 1), this.OverrideLayersRightEdge.Value));
                        }

                        if (directions.HasFlag(CardinalDirections.South) &&
                            (!this.OverrideLayersBottomEdge.IsEnabled || this.OverrideLayersBottomEdge.Value != Layers.None)) {
                            allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(1, 0), this.OverrideLayersBottomEdge.Value));
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

            var offset = this._tileable.WorldPosition - this._tileable.LocalPosition;
            foreach (var segment in allSegments) {
                var start = this._tileable.CurrentGrid.GetTilePosition(segment.StartPoint) - offset;
                var end = this._tileable.CurrentGrid.GetTilePosition(segment.EndPoint) - offset;

                var collider = new LineCollider(start, end) {
                    Layers = segment.Layers
                };

                collider.Initialize(this);
                this._colliders.Add(collider);
            }
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

    private void Tileable_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private class TileLineSegment {
        public TileLineSegment(Point firstPoint, Point secondPoint, Layers layers) {
            this.Layers = layers;

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

        public Point EndPoint { get; private set; }

        public bool IsHorizontal => this.StartPoint.Y == this.EndPoint.Y;

        public bool IsVertical => this.StartPoint.X == this.EndPoint.X;

        public Layers Layers { get; }

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
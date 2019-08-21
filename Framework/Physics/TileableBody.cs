namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A <see cref="BaseBody"/> which reacts to a <see cref="ITileable"/> parent and creates
    /// colliders based on the available grid.
    /// </summary>
    public sealed class TileableBody : BaseBody {
        private readonly List<Collider> _colliders = new List<Collider>();
        private ITileable _tileable;

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                return this._tileable?.BoundingArea ?? new BoundingArea();
            }
        }

        /// <inheritdoc/>
        public override bool HasCollider {
            get {
                return this._colliders.Any();
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<Collider> GetColliders() {
            return this._colliders;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this.ParentChanged += this.TileableBodyComponent_ParentChanged;
            this.SetTileable(this.Parent as ITileable);
        }

        private CardinalDirections GetEdgeDirections(Point tile) {
            var directionss = CardinalDirections.None;

            if (!this._tileable.HasActiveTileAt(tile - new Point(1, 0))) {
                directionss |= CardinalDirections.West;
            }

            if (!this._tileable.HasActiveTileAt(tile + new Point(0, 1))) {
                directionss |= CardinalDirections.North;
            }

            if (!this._tileable.HasActiveTileAt(tile + new Point(1, 0))) {
                directionss |= CardinalDirections.East;
            }

            if (!this._tileable.HasActiveTileAt(tile - new Point(0, 1))) {
                directionss |= CardinalDirections.South;
            }

            return directionss;
        }

        private void OnRequestReset(object sender, EventArgs e) {
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
                            if (directions.HasFlag(CardinalDirections.West)) {
                                allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(0, 1)));
                            }

                            if (directions.HasFlag(CardinalDirections.North)) {
                                allSegments.Add(new TileLineSegment(currentTile + new Point(0, 1), currentTile + new Point(1, 1)));
                            }

                            if (directions.HasFlag(CardinalDirections.East)) {
                                allSegments.Add(new TileLineSegment(currentTile + new Point(1, 0), currentTile + new Point(1, 1)));
                            }

                            if (directions.HasFlag(CardinalDirections.South)) {
                                allSegments.Add(new TileLineSegment(currentTile, currentTile + new Point(1, 0)));
                            }
                        }
                    }
                }

                var removedSegments = new List<TileLineSegment>();
                var horizontalSegmenets = allSegments.Where(x => x.IsHorizontal).OrderBy(x => x.StartPoint.X).ToList();
                foreach (var segment in horizontalSegmenets) {
                    if (!removedSegments.Contains(segment)) {
                        var compatibleSegments = horizontalSegmenets.Except(removedSegments).Where(x => x.StartPoint.Y == segment.StartPoint.Y).OrderBy(x => x.StartPoint.X).ToList();

                        foreach (var compatibleSegment in compatibleSegments) {
                            if (segment.TryCombineWith(compatibleSegment)) {
                                removedSegments.Add(compatibleSegment);
                            }
                        }
                    }
                }

                var verticalSegments = allSegments.Where(x => !x.IsHorizontal).OrderBy(x => x.StartPoint.Y).ToList();
                foreach (var segment in verticalSegments) {
                    if (!removedSegments.Contains(segment)) {
                        var compatibleSegments = verticalSegments.Except(removedSegments).Where(x => x.StartPoint.X == segment.StartPoint.X).OrderBy(x => x.StartPoint.Y).ToList();

                        foreach (var compatibleSegment in compatibleSegments) {
                            if (segment.TryCombineWith(compatibleSegment)) {
                                removedSegments.Add(compatibleSegment);
                            }
                        }
                    }
                }

                foreach (var removedSegment in removedSegments) {
                    allSegments.Remove(removedSegment);
                }

                var worldGrid = this._tileable.WorldGrid;
                foreach (var segment in allSegments) {
                    var collider = new LineCollider(worldGrid.GetTilePosition(segment.StartPoint), worldGrid.GetTilePosition(segment.EndPoint));
                    collider.Initialize(this);
                    this._colliders.Add(collider);
                }
            }
        }

        private void SetTileable(ITileable tileable) {
            if (this._tileable != null) {
                this._tileable.GridChanged -= this.OnRequestReset;
                this._tileable.TilesChanged -= this.OnRequestReset;
                this._tileable.TransformChanged -= this.OnRequestReset;
            }

            this._tileable = tileable;

            if (this._tileable != null) {
                this._tileable.GridChanged += this.OnRequestReset;
                this._tileable.TilesChanged += this.OnRequestReset;
                this._tileable.TransformChanged += this.OnRequestReset;
            }

            this.ResetColliders();
        }

        private void TileableBodyComponent_ParentChanged(object sender, BaseComponent e) {
            this.SetTileable(e as ITileable);
        }

        private class TileLineSegment {

            public TileLineSegment(Point firstPoint, Point secondPoint) {
                if (firstPoint.X == secondPoint.X) {
                    this.StartPoint = new Point(firstPoint.X, Math.Min(firstPoint.Y, secondPoint.Y));
                    this.EndPoint = new Point(firstPoint.X, Math.Max(firstPoint.Y, secondPoint.Y));
                }
                else if (firstPoint.Y == secondPoint.Y) {
                    this.StartPoint = new Point(Math.Min(firstPoint.X, secondPoint.X), firstPoint.Y);
                    this.EndPoint = new Point(Math.Max(firstPoint.X, secondPoint.X), firstPoint.Y);
                }
                else {
                    throw new ArgumentOutOfRangeException();
                }
            }

            public Point EndPoint { get; private set; }

            public bool IsHorizontal {
                get {
                    return this.StartPoint.Y == this.EndPoint.Y;
                }
            }

            public Point StartPoint { get; private set; }

            public bool TryCombineWith(TileLineSegment otherLine) {
                var result = false;
                if (!this.IsHorizontal) {
                    if (this.StartPoint.Y == otherLine.EndPoint.Y || this.EndPoint.Y == otherLine.StartPoint.Y) {
                        this.StartPoint = new Point(this.StartPoint.X, Math.Min(this.StartPoint.Y, otherLine.StartPoint.Y));
                        this.EndPoint = new Point(this.StartPoint.X, Math.Max(this.EndPoint.Y, otherLine.EndPoint.Y));
                        result = true;
                    }
                }
                else {
                    if (this.StartPoint.X == otherLine.EndPoint.X || this.EndPoint.X == otherLine.StartPoint.X) {
                        this.StartPoint = new Point(Math.Min(this.StartPoint.X, otherLine.StartPoint.X), this.StartPoint.Y);
                        this.EndPoint = new Point(Math.Max(this.EndPoint.X, otherLine.EndPoint.X), this.StartPoint.Y);
                        result = true;
                    }
                }

                return result;
            }
        }
    }
}
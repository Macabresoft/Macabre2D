namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Collider representing a generic polygon to be used by the physics engine.
    /// </summary>
    /// <seealso cref="Collider" />
    public class PolygonCollider : Collider {
        private readonly ResettableLazy<Vector2> _center;
        private readonly ResettableLazy<List<Vector2>> _normals;

        [DataMember]
        private readonly List<Vector2> _vertices = new();

        private readonly ResettableLazy<List<Vector2>> _worldPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
        /// </summary>
        public PolygonCollider() : base() {
            this._worldPoints = new ResettableLazy<List<Vector2>>(this.CreateWorldPoints);
            this._normals = new ResettableLazy<List<Vector2>>(this.GetNormals);
            this._center = new ResettableLazy<Vector2>(() => this.WorldPoints.GetAverage());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public PolygonCollider(IEnumerable<Vector2> points) : this() {
            this._vertices.AddRange(points);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public PolygonCollider(params Vector2[] points) : this(points as IEnumerable<Vector2>) {
        }

        /// <inheritdoc />
        public override ColliderType ColliderType => ColliderType.Polygon;

        /// <summary>
        /// Gets the normals.
        /// </summary>
        /// <value>The normals.</value>
        public IReadOnlyCollection<Vector2> Normals => this._normals.Value;

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        public IReadOnlyList<Vector2> Vertices => this._vertices;

        /// <summary>
        /// Gets the world points.
        /// </summary>
        /// <value>The world points.</value>
        public IReadOnlyCollection<Vector2> WorldPoints => this._worldPoints.Value;

        /// <inheritdoc />
        public override bool Contains(Vector2 point) {
            if (!this.BoundingArea.Contains(point)) {
                return false;
            }

            var positiveCount = 0;
            var negativeCount = 0;

            for (var i = 0; i < this.WorldPoints.Count; i++) {
                var current = this.WorldPoints.ElementAt(i);

                // We do not consider it containment if a point is along the outside edge.
                if (current == point) {
                    return false;
                }

                var next = this.GetNextWorldPoint(i);
                var crossProduct = (point.X - current.X) * (next.Y - current.Y) - (point.Y - current.Y) * (next.X - current.X);

                if (crossProduct > 0f) {
                    positiveCount++;
                }
                else if (crossProduct < 0f) {
                    negativeCount++;
                }

                // If the sign changes, that means the point is outside of the polygon
                if (positiveCount > 0 && negativeCount > 0) {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Contains(Collider other) {
            if (!this.BoundingArea.Overlaps(other.BoundingArea)) {
                return false;
            }

            if (other is CircleCollider circle) {
                // Quickly exit if the center of the circle isn't even inside this polygon.
                if (!this.Contains(circle.Center)) {
                    return false;
                }

                for (var i = 0; i < this.WorldPoints.Count; i++) {
                    var current = this.WorldPoints.ElementAt(i);

                    // Touching edges is not containment
                    if (Vector2.Distance(circle.Center, current) <= circle.ScaledRadius) {
                        return false;
                    }

                    var next = this.GetNextWorldPoint(i);

                    // If current and next are the same, there's no point in performing the rest of
                    // the operations (in fact, it might be downright dangerous with the division
                    // that occurs later).
                    if (current == next) {
                        continue;
                    }

                    // For each edge of the polygon, we want to find the closest point on the edge
                    // to the center of the circle. If the distance between this new point and the
                    // center of the circle is less than the circle's radius, we know the circle is
                    // not inside this collider.
                    var edge = next - current;
                    var currentToCenter = circle.Center - current;
                    var distance = Vector2.Dot(currentToCenter, edge) / edge.LengthSquared();
                    var pointOnLine = current + edge * distance;

                    if (Vector2.Distance(circle.Center, pointOnLine) <= circle.ScaledRadius) {
                        return false;
                    }
                }

                return true;
            }

            if (other is PolygonCollider polygon) {
                foreach (var point in polygon.WorldPoints) {
                    if (!this.Contains(point)) {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a rectangle from the specified height and width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The rectangle polygon collider.</returns>
        public static PolygonCollider CreateRectangle(float width, float height) {
            var points = new List<Vector2>();
            var halfWidth = 0.5f * width;
            var halfHeight = 0.5f * height;
            points.Add(new Vector2(-halfWidth, -halfHeight));
            points.Add(new Vector2(-halfWidth, halfHeight));
            points.Add(new Vector2(halfWidth, halfHeight));
            points.Add(new Vector2(halfWidth, -halfHeight));
            return new PolygonCollider(points);
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<Vector2> GetAxesForSat(Collider other) {
            return this.Normals;
        }

        /// <inheritdoc />
        public override Vector2 GetCenter() {
            return this._center.Value;
        }

        /// <inheritdoc />
        public override Projection GetProjection(Vector2 axis) {
            return Projection.CreatePolygonProjection(axis, this.WorldPoints);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public override void Reset() {
            this.ForceClockwise();
            base.Reset();
        }

        /// <inheritdoc />
        protected override BoundingArea CreateBoundingArea() {
            return new(
                this.WorldPoints.Min(p => p.X),
                this.WorldPoints.Max(p => p.X),
                this.WorldPoints.Min(p => p.Y),
                this.WorldPoints.Max(p => p.Y));
        }

        /// <summary>
        /// Gets the next world point based on the given index.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>Gets the next world point.</returns>
        protected Vector2 GetNextWorldPoint(int current) {
            return this._worldPoints.Value[current + 1 == this.WorldPoints.Count ? 0 : current + 1];
        }

        /// <summary>
        /// Gets this polygon's normals.
        /// </summary>
        /// <returns>This polygon's normals.</returns>
        protected virtual List<Vector2> GetNormals() {
            var normals = new List<Vector2>();

            for (var i = 0; i < this.WorldPoints.Count; i++) {
                normals.Add(this.GetNormal(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i)));
            }

            return normals;
        }

        /// <inheritdoc />
        protected override void ResetLazyFields() {
            base.ResetLazyFields();
            this._worldPoints.Reset();
            this._normals.Reset();
            this._center.Reset();
        }

        /// <summary>
        /// Clears the vertex collection and adds the newly provided vertices.
        /// </summary>
        /// <param name="vertices">The new vertices.</param>
        protected void ResetVertices(IEnumerable<Vector2> vertices) {
            this._vertices.Clear();
            this._vertices.AddRange(vertices);
            this.Reset();
        }

        /// <inheritdoc />
        protected override bool TryHit(LineSegment ray, out RaycastHit result) {
            var hasIntersection = false;
            var contactPoint = Vector2.Zero;
            var normal = Vector2.Zero;
            var shortestDistance = float.MaxValue;

            for (var i = 0; i < this.WorldPoints.Count; i++) {
                var edge = new LineSegment(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i));

                if (ray.Intersects(edge, out var intersection)) {
                    var currentDistance = Vector2.Distance(ray.Start, intersection);

                    if (currentDistance < shortestDistance) {
                        contactPoint = intersection;
                        normal = edge.GetNormal();
                        shortestDistance = currentDistance;
                        hasIntersection = true;
                    }
                }
            }

            result = hasIntersection ? new RaycastHit(this, contactPoint, normal) : RaycastHit.Empty;
            return hasIntersection;
        }

        /// <summary>
        /// Tries to set the vertex at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="vertex">The vertex.</param>
        /// <returns>A value indicating whether or not the vertex was set at the index.</returns>
        protected bool TrySetVertex(int index, Vector2 vertex) {
            var result = false;
            if (index >= 0 && index < this._vertices.Count) {
                this._vertices[index] = vertex;
                this.Reset();
                result = true;
            }

            return result;
        }

        private List<Vector2> CreateWorldPoints() {
            var worldPoints = new List<Vector2>();
            if (this.Body != null) {
                foreach (var point in this._vertices) {
                    var worldPoint = this.Body.GetWorldTransform(this.Offset + point).Position;
                    worldPoints.Add(worldPoint);
                }
            }

            return worldPoints;
        }

        private void ForceClockwise() {
            var sum = 0f;

            for (var i = 0; i < this._vertices.Count; i++) {
                var current = this._vertices[i];
                var next = this._vertices[i + 1 == this._vertices.Count ? 0 : i + 1];

                sum += current.X * next.Y - next.X * current.Y;
            }

            if (sum < 0f) {
                this._vertices.Reverse();
            }
        }
    }
}
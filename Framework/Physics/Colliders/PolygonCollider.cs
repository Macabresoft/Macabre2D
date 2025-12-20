namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

/// <summary>
/// Collider representing a generic polygon to be used by the physics engine.
/// </summary>
/// <seealso cref="Collider" />
public abstract class PolygonCollider : Collider {
    private static readonly Vector2 HorizontalNormal = new(0f, 1f);
    private readonly ResettableLazy<Vector2> _center;
    private readonly ResettableLazy<List<Vector2>> _normals;

    [DataMember]
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly List<Vector2> _vertices = [];

    private readonly ResettableLazy<List<Vector2>> _worldPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
    /// </summary>
    protected PolygonCollider() : base() {
        this._worldPoints = new ResettableLazy<List<Vector2>>(this.CreateWorldPoints);
        this._normals = new ResettableLazy<List<Vector2>>(this.GetNormals);
        this._center = new ResettableLazy<Vector2>(() => this.WorldPoints.GetAverage());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
    /// </summary>
    /// <param name="numberOfVertices">The number of vertices.</param>
    protected PolygonCollider(int numberOfVertices) : this() {
        if (numberOfVertices == 0) {
            this._vertices.Clear();
        }
        else if (this._vertices.Count > numberOfVertices) {
            this._vertices.RemoveRange(numberOfVertices, this._vertices.Count - numberOfVertices);
        }
        else if (this._vertices.Count < numberOfVertices) {
            for (var i = this._vertices.Count; i < numberOfVertices; i++) {
                this._vertices.Add(Vector2.Zero);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
    /// </summary>
    /// <param name="vertices">The vertices.</param>
    protected PolygonCollider(IEnumerable<Vector2> vertices) : this() {
        this._vertices.Clear();
        this._vertices.AddRange(vertices);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonCollider" /> class.
    /// </summary>
    /// <param name="points">The points.</param>
    protected PolygonCollider(params Vector2[] points) : this(points as IEnumerable<Vector2>) {
    }

    /// <summary>
    /// Gets a value indicating whether the first and final vertex should be connected.
    /// </summary>
    public virtual bool ConnectFirstAndFinalVertex => true;

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
                if (Vector2.Distance(circle.Center, current) <= circle.Radius) {
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

                if (Vector2.Distance(circle.Center, pointOnLine) <= circle.Radius) {
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

    /// <inheritdoc />
    public override Vector2 GetCenter() => this._center.Value;

    /// <inheritdoc />
    public override bool Intersects(BoundingArea boundingArea, out IEnumerable<RaycastHit> hits) {
        if (this.BoundingArea.Overlaps(boundingArea)) {
            var actualHits = new List<RaycastHit>();
            var lineSegments = this.GetLineSegments(boundingArea);

            foreach (var lineSegment in lineSegments) {
                for (var i = 0; i < this.WorldPoints.Count; i++) {
                    var edge = new LineSegment(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i));

                    if (lineSegment.Intersects(edge, out var contactPoint)) {
                        actualHits.Add(new RaycastHit(this, contactPoint, edge.GetNormal()));
                    }
                }
            }

            foreach (var point in this.WorldPoints.Where(boundingArea.Contains)) {
                actualHits.Add(new RaycastHit(this, point, HorizontalNormal));
            }

            if (actualHits.Any()) {
                hits = actualHits;
                return true;
            }
        }

        hits = [];
        return false;
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
        if (this.WorldPoints.Any()) {
            return new BoundingArea(
                this.WorldPoints.Min(p => p.X),
                this.WorldPoints.Max(p => p.X),
                this.WorldPoints.Min(p => p.Y),
                this.WorldPoints.Max(p => p.Y));
        }

        return BoundingArea.Empty;
    }

    /// <inheritdoc />
    protected override IReadOnlyCollection<Vector2> GetAxesForSat(Collider other) => this.Normals;

    /// <summary>
    /// Gets the next world point based on the given index.
    /// </summary>
    /// <param name="current">The current.</param>
    /// <returns>Gets the next world point.</returns>
    protected Vector2 GetNextWorldPoint(int current) => this._worldPoints.Value[current + 1 == this.WorldPoints.Count ? 0 : current + 1];

    /// <summary>
    /// Gets this polygon's normals.
    /// </summary>
    /// <returns>This polygon's normals.</returns>
    protected virtual List<Vector2> GetNormals() {
        var normals = new List<Vector2>();
        var count = this.ConnectFirstAndFinalVertex ? this.WorldPoints.Count : this.WorldPoints.Count - 1;

        for (var i = 0; i < count; i++) {
            normals.Add(this.GetNormal(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i)));
        }

        return normals;
    }

    /// <inheritdoc />
    protected override Projection GetProjection(Vector2 axis) => Projection.CreatePolygonProjection(axis, this.WorldPoints);

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
    /// <param name="performFullReset">A value indicating whether a full reset should be performed. Mark this false if calling ResetVertices from the Reset method.</param>
    protected void ResetVertices(IEnumerable<Vector2> vertices, bool performFullReset) {
        this._vertices.Clear();
        this._vertices.AddRange(vertices);

        if (performFullReset) {
            this.Reset();
        }
    }

    /// <inheritdoc />
    protected override bool TryHit(LineSegment ray, out RaycastHit result) {
        var hasIntersection = false;
        var contactPoint = Vector2.Zero;
        var normal = Vector2.Zero;
        var shortestDistance = float.MaxValue;
        var count = this.ConnectFirstAndFinalVertex ? this.WorldPoints.Count : this.WorldPoints.Count - 1;

        for (var i = 0; i < count; i++) {
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

    private List<Vector2> CreateWorldPoints() {
        var worldPoints = new List<Vector2>();
        if (this.Body != null) {
            worldPoints.AddRange(this._vertices.Select(point => this.Body.GetWorldPosition(this.Offset + point)));
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
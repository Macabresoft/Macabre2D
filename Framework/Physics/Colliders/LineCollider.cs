namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Collider representing a line defined by a start and end point to be used by the physics engine.
/// </summary>
public sealed class LineCollider : PolygonCollider {
    /// <summary>
    /// Initializes a new instance of the <see cref="LineCollider" /> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public LineCollider(Vector2 start, Vector2 end) : base(start, end) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineCollider" /> class.
    /// </summary>
    public LineCollider() : base(2) {
    }

    /// <inheritdoc />
    public override bool ConnectFirstAndFinalVertex => false;

    /// <summary>
    /// Gets or sets the end. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    /// <value>The end.</value>
    [DataMember(Order = 1)]
    public Vector2 End {
        get => this.Vertices[1];
        set {
            this.ResetVertices([this.Start, value], this.Body != null);
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the start. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    /// <value>The start.</value>
    [DataMember(Order = 0)]
    public Vector2 Start {
        get => this.Vertices[0];
        set {
            this.ResetVertices([value, this.End], this.Body != null);
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public override bool Contains(Collider other) => false;

    /// <inheritdoc />
    public override bool Contains(Vector2 point) => false;

    /// <inheritdoc />
    protected override IReadOnlyCollection<Vector2> GetAxesForSat(Collider other) {
        var normals = base.GetAxesForSat(other);
        var result = new List<Vector2>();
        var thisCenter = this.GetCenter();
        var otherCenter = other.GetCenter();

        foreach (var normal in normals) {
            if (Vector2.Distance(thisCenter + normal, otherCenter) < Vector2.Distance(thisCenter - normal, otherCenter)) {
                result.Add(normal);
            }
            else {
                result.Add(-normal);
            }
        }

        return result;
    }

    /// <inheritdoc />
    protected override List<Vector2> GetNormals() =>
        [this.GetNormal(this.WorldPoints.ElementAt(0), this.WorldPoints.ElementAt(1))];

    /// <inheritdoc />
    protected override bool TryHit(LineSegment ray, out RaycastHit result) {
        var edge = new LineSegment(this.WorldPoints.ElementAt(0), this.WorldPoints.ElementAt(1));

        if (ray.Intersects(edge, out var intersection)) {
            var normal = edge.GetNormal();

            // We need to reverse the normal if the start of the ray is in the opposite
            // direction of our default normal
            if (Vector2.Distance(intersection - normal, ray.Start) < Vector2.Distance(intersection + normal, ray.Start)) {
                normal = -normal;
            }

            result = new RaycastHit(this, intersection, normal);
            return true;
        }

        result = RaycastHit.Empty;
        return false;
    }
}
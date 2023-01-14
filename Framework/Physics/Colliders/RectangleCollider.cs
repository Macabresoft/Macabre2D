namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Collider representing a rectangle to be used by the physics engine.
/// </summary>
[Display(Name = "Rectangle Collider")]
public sealed class RectangleCollider : PolygonCollider {
    private const int MaximumIndex = 2;
    private const int MinimumIndex = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleCollider" /> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public RectangleCollider(Vector2 minimum, Vector2 maximum) : base(CreateVertices(minimum, maximum)) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RectangleCollider" />.
    /// </summary>
    public RectangleCollider() : base() {
    }

    /// <summary>
    /// Gets or sets the minimum vertex on the rectangle.
    /// </summary>
    [DataMember]
    public Vector2 Maximum {
        get {
            if (this.Vertices.Count == 4) {
                return this.Vertices[MaximumIndex];
            }

            return Vector2.Zero;
        }

        set {
            if (value != this.Maximum) {
                this.ResetVertices(CreateVertices(this.Minimum, value), true);
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the minimum vertex on the rectangle.
    /// </summary>
    [DataMember]
    public Vector2 Minimum {
        get {
            if (this.Vertices.Count == 4) {
                return this.Vertices[MinimumIndex];
            }

            return Vector2.Zero;
        }

        set {
            this.ResetVertices(CreateVertices(value, this.Maximum), true);
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public override bool Intersects(BoundingArea boundingArea, out IEnumerable<RaycastHit> hits) {
        var result = base.Intersects(boundingArea, out hits);
        return result || this.BoundingArea.Overlaps(boundingArea); // Temporary fix
    }

    /// <inheritdoc />
    protected override List<Vector2> GetNormals() {
        var normals = new List<Vector2>();

        // Since half the edges are parallel, we can optimize and only get half the normals.
        for (var i = 0; i < 2; i++) {
            normals.Add(this.GetNormal(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i)));
        }

        return normals;
    }

    private static IEnumerable<Vector2> CreateVertices(Vector2 minimum, Vector2 maximum) {
        return new[] {
            minimum,
            new Vector2(minimum.X, maximum.Y),
            maximum,
            new Vector2(maximum.X, minimum.Y)
        };
    }
}
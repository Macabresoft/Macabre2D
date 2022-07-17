namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A strip of lines as a collider.
/// </summary>
[Display(Name = "Line Strip Collider")]
public class LineStripCollider : PolygonCollider {
    [DataMember]
    private readonly RelativeVertices _relativeVertices = new();
    private readonly List<LineSegment> _lineSegments = new();
    private bool _isResetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    public LineStripCollider() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public LineStripCollider(IEnumerable<Vector2> points) : this() {
        this._relativeVertices.Reset(points);
    }

    /// <summary>
    /// Gets the line segments that make up this collider.
    /// </summary>
    public IReadOnlyCollection<LineSegment> LineSegments => this._lineSegments;

    /// <summary>
    /// Tries to get a line segment containing the provided point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="foundLineSegment">The found line segment.</param>
    /// <returns>A value indicating whether or not a matching line segment was found.</returns>
    public bool TryGetLineSegmentContainingPoint(Vector2 point, out LineSegment foundLineSegment) {
        foundLineSegment = LineSegment.Empty;

        foreach (var lineSegment in this.LineSegments) {
            if (lineSegment.Contains(point)) {
                foundLineSegment = lineSegment;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool ConnectFirstAndFinalVertex => false;
    
    /// <inheritdoc />
    public override bool Contains(Collider other) {
        return false;
    }

    /// <inheritdoc />
    public override bool Contains(Vector2 point) {
        return false;
    }

    public override void Reset() {
        this._relativeVertices.CollectionChanged -= this.RelativeVerticesCollectionChanged;

        if (!this._isResetting) {
            this.ResetVertices(this._relativeVertices.ToAbsolute(), false);
            base.Reset();
        }
        else {
            base.Reset();
        }
        
        this._lineSegments.Clear();

        if (this.WorldPoints.Count >= 2) {
            for (var i = 0; i < this.WorldPoints.Count - 1; i++) {
                this._lineSegments.Add(new LineSegment(this.WorldPoints.ElementAt(i), this.WorldPoints.ElementAt(i + 1)));
            }
        }

        this._relativeVertices.CollectionChanged += this.RelativeVerticesCollectionChanged;
    }

    private void RelativeVerticesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (this.Body != null && !this._isResetting) {
            try {
                this._isResetting = true;
                this.ResetVertices(this._relativeVertices.ToAbsolute(), this.Body != null);
            }
            finally {
                this._isResetting = false;
            }
        }
    }
}
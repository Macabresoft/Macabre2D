namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A strip of lines as a collider.
/// </summary>
public class LineStripCollider : PolygonCollider {
    [DataMember]
    private readonly RelativeVertices _relativeVertices = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    public LineStripCollider() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public LineStripCollider(IEnumerable<Vector2> points) : base() {
        this._relativeVertices.Reset(points);
        this._relativeVertices.CollectionChanged += this.RelativeVerticesCollectionChanged;
    }

    private void RelativeVerticesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (this.Body != null) {
            this.ResetVertices(this._relativeVertices.ToAbsolute(), true);
        }
    }
}
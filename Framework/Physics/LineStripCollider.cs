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
    private readonly LineStrip _lineStrip = new();

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
        this._lineStrip.Reset(points);
    }

    /// <inheritdoc />
    public override void Reset() {
        this.ResetVertices(this._lineStrip.ToAbsolute(), false);
        this._lineStrip.CollectionChanged += this.LineStrip_CollectionChanged;
        base.Reset();
    }

    private void LineStrip_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (this.Body != null) {
            this.ResetVertices(this._lineStrip.ToAbsolute(), true);
        }
    }
}
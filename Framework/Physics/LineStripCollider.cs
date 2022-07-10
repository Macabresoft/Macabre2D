namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A strip of lines as a collider.
/// </summary>
[Display(Name = "Line Strip Collider")]
public class LineStripCollider : PolygonCollider {
    [DataMember]
    private readonly RelativeVertices _relativeVertices = new();

    private bool _isResetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    public LineStripCollider() : base() {
        this._relativeVertices.CollectionChanged += this.RelativeVerticesCollectionChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripCollider" /> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public LineStripCollider(IEnumerable<Vector2> points) : this() {
        this._relativeVertices.Reset(points);
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
        if (!this._isResetting) {
            this.ResetVertices(this._relativeVertices.ToAbsolute(), false);
            base.Reset();
        }
        else {
            base.Reset();
        }
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
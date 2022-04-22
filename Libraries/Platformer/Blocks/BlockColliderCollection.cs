namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Collections;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A collection of colliders for blocks.
/// </summary>
public class BlockColliderCollection : IEnumerable<Collider> {
    private readonly List<LineCollider> _colliders = new() {
        new LineCollider(), new LineCollider(), new LineCollider(), new LineCollider()
    };

    /// <summary>
    /// Gets the bottom collider.
    /// </summary>
    public LineCollider BottomCollider {
        get => this._colliders[3];
    }

    /// <summary>
    /// Gets the left collider.
    /// </summary>
    public LineCollider LeftCollider {
        get => this._colliders[0];
    }

    /// <summary>
    /// Gets the right collider.
    /// </summary>
    public LineCollider RightCollider {
        get => this._colliders[2];
    }

    /// <summary>
    /// Gets the top collider.
    /// </summary>
    public LineCollider TopCollider {
        get => this._colliders[1];
    }

    /// <inheritdoc />
    public IEnumerator<Collider> GetEnumerator() {
        return this._colliders.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return this._colliders.GetEnumerator();
    }
}
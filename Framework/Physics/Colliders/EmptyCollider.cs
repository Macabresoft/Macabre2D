namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// An empty collider.
/// </summary>
public class EmptyCollider : Collider {
    /// <inheritdoc />
    public override ColliderType ColliderType => ColliderType.Empty;

    /// <inheritdoc />
    public override bool Contains(Collider other) {
        return false;
    }

    /// <inheritdoc />
    public override bool Contains(Vector2 point) {
        return false;
    }

    /// <inheritdoc />
    public override Vector2 GetCenter() {
        return Vector2.Zero;
    }

    /// <inheritdoc />
    protected override BoundingArea CreateBoundingArea() {
        return BoundingArea.Empty;
    }

    /// <inheritdoc />
    protected override IReadOnlyCollection<Vector2> GetAxesForSat(Collider other) {
        return Array.Empty<Vector2>();
    }

    /// <inheritdoc />
    protected override Projection GetProjection(Vector2 axis) {
        return Projection.Empty;
    }

    /// <inheritdoc />
    protected override bool TryHit(LineSegment ray, out RaycastHit result) {
        result = RaycastHit.Empty;
        return false;
    }
}
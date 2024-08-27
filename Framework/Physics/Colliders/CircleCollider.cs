namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Collider representing a circle to be used by the physics engine.
/// </summary>
/// <seealso cref="Collider" />
public sealed class CircleCollider : Collider {
    private float _radius = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleCollider" /> class.
    /// </summary>
    public CircleCollider() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleCollider" /> class.
    /// </summary>
    /// <param name="radius">The radius.</param>
    public CircleCollider(float radius) : this() {
        this.Radius = radius;
    }

    /// <summary>
    /// Gets the center.
    /// </summary>
    /// <value>The center.</value>
    public Vector2 Center => this.WorldPosition;

    /// <inheritdoc />
    public override ColliderType ColliderType => ColliderType.Circle;

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    [DataMember]
    public float Radius {
        get => this._radius;
        set {
            this._radius = value;
            this.ResetLazyFields();
        }
    }

    /// <inheritdoc />
    public override bool Contains(Vector2 point) {
        // Do we want to do <= here?
        return Vector2.Distance(point, this.Center) <= this.Radius;
    }

    /// <inheritdoc />
    public override bool Contains(Collider other) {
        if (other is CircleCollider circle) {
            var distance = Vector2.Distance(this.Center, circle.Center);
            return this.Radius > distance + circle.Radius; // Should this be >= ?
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
    public override Vector2 GetCenter() {
        return this.Center;
    }

    /// <inheritdoc />
    protected override BoundingArea CreateBoundingArea() {
        return new BoundingArea(
            this.Center.X - this.Radius,
            this.Center.X + this.Radius,
            this.Center.Y - this.Radius,
            this.Center.Y + this.Radius);
    }

    /// <inheritdoc />
    protected override IReadOnlyCollection<Vector2> GetAxesForSat(Collider other) {
        var axes = new List<Vector2>();

        if (other is PolygonCollider polygon) {
            var distance = float.MaxValue;
            var closestPoint = polygon.WorldPoints.First();
            foreach (var point in polygon.WorldPoints) {
                // Find the closest point on the polygon to the circle
                var currentDistance = Vector2.Distance(point, this.Center);
                if (currentDistance < distance) {
                    distance = currentDistance;
                    closestPoint = point;
                }
            }

            var axis = new Vector2(closestPoint.X - this.Center.X, closestPoint.Y - this.Center.Y);
            axis.Normalize();

            axes.Add(axis);
        }
        else if (other is CircleCollider circle) {
            var axis = this.Center - circle.Center;

            if (axis != Vector2.Zero) {
                axis.Normalize();
            }
            else {
                axis = new Vector2(0f, 1f);
            }

            axes.Add(axis);
        }

        return axes;
    }

    /// <inheritdoc />
    protected override Projection GetProjection(Vector2 axis) {
        var minimum = Vector2.Dot(axis, this.Center);
        var maximum = minimum + this.Radius;
        minimum -= this.Radius;
        return new Projection(axis, minimum, maximum);
    }

    /// <inheritdoc />
    protected override bool TryHit(LineSegment ray, out RaycastHit result) {
        var distanceX = ray.Direction.X * ray.Distance;
        var distanceY = ray.Direction.Y * ray.Distance;

        var valueA = (float)(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

        if (valueA <= 0.0000001f) {
            result = RaycastHit.Empty;
            return false;
        }

        var valueB = 2f * (distanceX * (ray.Start.X - this.Center.X) + distanceY * (ray.Start.Y - this.Center.Y));
        var valueC = Math.Pow(ray.Start.X - this.Center.X, 2f) + Math.Pow(ray.Start.Y - this.Center.Y, 2f) - Math.Pow(this.Radius, 2f);
        var det = Math.Pow(valueB, 2f) - 4f * valueA * valueC;

        if (det < 0f) {
            result = RaycastHit.Empty;
            return false;
        }

        if (det == 0f) {
            var t = -valueB / (2f * valueA);
            var intersection = new Vector2(ray.Start.X + t * distanceX, ray.Start.Y + t * distanceY);
            var normal = intersection - this.Center;
            normal.Normalize();
            result = new RaycastHit(this, intersection, normal);
        }
        else {
            var t = (float)((-valueB + Math.Sqrt(det)) / (2f * valueA));
            var intersectionA = new Vector2(ray.Start.X + t * distanceX, ray.Start.Y + t * distanceY);
            t = (float)((-valueB - Math.Sqrt(det)) / (2f * valueA));
            var intersectionB = new Vector2(ray.Start.X + t * distanceX, ray.Start.Y + t * distanceY);

            if (Vector2.Distance(ray.Start, intersectionA) < Vector2.Distance(ray.Start, intersectionB)) {
                var normal = intersectionA - this.Center;
                normal.Normalize();
                result = new RaycastHit(this, intersectionA, normal);
            }
            else {
                var normal = intersectionB - this.Center;
                normal.Normalize();
                result = new RaycastHit(this, intersectionB, normal);
            }
        }

        return true;
    }
}
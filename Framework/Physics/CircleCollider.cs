namespace Macabre2D.Framework.Physics {

    using Macabre2D.Framework.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeration which defines how a circle's radius will scale when its body has a scale other
    /// than (1, 1). This is necessary, because circle colliders need to stay circles. They cannot
    /// become any other ellipse.
    /// </summary>
    public enum RadiusScalingType {

        /// <summary>
        /// No scaling will occur.
        /// </summary>
        None = 0,

        /// <summary>
        /// The circle's radius will scale on the body's X scale value.
        /// </summary>
        X = 1,

        /// <summary>
        /// The circle's radius will scale on the body's Y scale value.
        /// </summary>
        Y = 2,

        /// <summary>
        /// The circle's radius will scale on an average of the body's X and Y scale values.
        /// </summary>
        Average = 3
    }

    /// <summary>
    /// Collider representing a circle to be used by the physics engine.
    /// </summary>
    /// <seealso cref="Collider"/>
    public sealed class CircleCollider : Collider {

        [DataMember]
        private float _radius;

        [DataMember]
        private RadiusScalingType _radiusScalingType = RadiusScalingType.None;

        private Lazy<float> _scaledRadius;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollider"/> class.
        /// </summary>
        public CircleCollider() : base() {
            this._scaledRadius = this._scaledRadius.Reset(this.CreateScaledRadius);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollider"/> class.
        /// </summary>
        /// <param name="radius">The radius.</param>
        public CircleCollider(float radius) : this() {
            this.Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollider"/> class.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="scalingType">Type of the scaling.</param>
        public CircleCollider(float radius, RadiusScalingType scalingType) : this(radius) {
            this._radiusScalingType = scalingType;
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <value>The center.</value>
        public Vector2 Center {
            get {
                return this.Transform.Position;
            }
        }

        /// <inheritdoc/>
        public override ColliderType ColliderType {
            get {
                return ColliderType.Circle;
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius {
            get {
                return this._radius;
            }

            set {
                this._radius = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the radius scaling.
        /// </summary>
        /// <value>The type of the radius scaling.</value>
        public RadiusScalingType RadiusScalingType {
            get {
                return this._radiusScalingType;
            }

            set {
                if (this._radiusScalingType != value) {
                    this._radiusScalingType = value;
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets the radius of this circle after all scaling operations have occurred.
        /// </summary>
        /// <value>The scaled radius.</value>
        public float ScaledRadius {
            get {
                return this._scaledRadius.Value;
            }
        }

        /// <inheritdoc/>
        public override bool Contains(Vector2 point) {
            // Do we want to do <= here?
            return Vector2.Distance(point, this.Center) <= this.ScaledRadius;
        }

        /// <inheritdoc/>
        public override bool Contains(Collider other) {
            if (other.ColliderType == ColliderType.Circle) {
                var circle = other as CircleCollider;
                var distance = Vector2.Distance(this.Center, circle.Center);
                return this.ScaledRadius > (distance + circle.ScaledRadius); // Should this be >= ?
            }
            else if (other.ColliderType == ColliderType.Polygon) {
                var polygon = other as PolygonCollider;
                foreach (var point in polygon.WorldPoints) {
                    if (!this.Contains(point)) {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override List<Vector2> GetAxesForSAT(Collider other) {
            var axes = new List<Vector2>();

            if (other is PolygonCollider polygon) {
                // Find the closest point on the polygon to the circle
                var currentDistance = 0f;
                var distance = float.MaxValue;
                var closestPoint = polygon.WorldPoints.First();
                foreach (var point in polygon.WorldPoints) {
                    currentDistance = Vector2.Distance(point, this.Center);
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

        /// <inheritdoc/>
        public override Vector2 GetCenter() {
            return this.Center;
        }

        /// <inheritdoc/>
        public override Projection GetProjection(Vector2 axis) {
            var minimum = Vector2.Dot(axis, this.Center);
            var maximum = minimum + this.ScaledRadius;
            minimum -= this.ScaledRadius;
            return new Projection(axis, minimum, maximum);
        }

        internal override void Reset() {
            base.Reset();
            this._scaledRadius = this._scaledRadius.Reset(this.CreateScaledRadius);
        }

        /// <inheritdoc/>
        protected override BoundingArea CreateBoundingArea() {
            return new BoundingArea(
                this.Center.X - this.ScaledRadius,
                this.Center.X + this.ScaledRadius,
                this.Center.Y - this.ScaledRadius,
                this.Center.Y + this.ScaledRadius);
        }

        /// <inheritdoc/>
        protected override bool TryHit(LineSegment ray, out RaycastHit result) {
            var distanceX = ray.Direction.X * ray.Distance;
            var distanceY = ray.Direction.Y * ray.Distance;

            var valueA = (float)(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));

            if (valueA <= 0.0000001f) {
                result = null;
                return false;
            }

            var valueB = 2f * (distanceX * (ray.Start.X - this.Center.X) + distanceY * (ray.Start.Y - this.Center.Y));
            var valueC = Math.Pow(ray.Start.X - this.Center.X, 2f) + Math.Pow(ray.Start.Y - this.Center.Y, 2f) - Math.Pow(this.ScaledRadius, 2f);
            var det = Math.Pow(valueB, 2f) - 4f * valueA * valueC;

            if (det < 0f) {
                result = null;
                return false;
            }
            else if (det == 0f) {
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

        private float CreateScaledRadius() {
            var worldTransform = this.Body.WorldTransform;
            switch (this.RadiusScalingType) {
                case RadiusScalingType.X:
                    return this._radius * worldTransform.Scale.X;

                case RadiusScalingType.Y:
                    return this._radius * worldTransform.Scale.Y;

                case RadiusScalingType.Average:
                    return this._radius * 0.5f * (worldTransform.Scale.X + worldTransform.Scale.Y);
            }

            return this._radius;
        }
    }
}
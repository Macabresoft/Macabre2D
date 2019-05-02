namespace Macabre2D.Framework.Physics {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The type of collider being used.
    /// </summary>
    public enum ColliderType {

        /// <summary>
        /// Circle collider.
        /// </summary>
        Circle,

        /// <summary>
        /// Polygon collider.
        /// </summary>
        Polygon,

        /// <summary>
        /// Some other type of collider. Most custom colliders will fall into this category.
        /// </summary>
        Other
    }

    /// <summary>
    /// Collider to be attached to bodies in the physics engine.
    /// </summary>
    [DataContract]
    public abstract class Collider : IBoundable {
        private Lazy<BoundingArea> _boundingArea;

        [DataMember]
        private Vector2 _offset;

        private Lazy<Transform> _transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="Collider"/> class.
        /// </summary>
        public Collider() {
            this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
        }

        /// <summary>
        /// Gets the body that this collider is attached to.
        /// </summary>
        /// <value>The body.</value>
        public Body Body { get; private set; }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets the type of the collider.
        /// </summary>
        /// <value>The type of the collider.</value>
        public abstract ColliderType ColliderType { get; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector2 Offset {
            get {
                return this._offset;
            }

            set {
                if (this._offset != value) {
                    this._offset = value;
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <value>The transform.</value>
        internal Transform Transform {
            get {
                return this._transform.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this instance collides with the specified collider.
        /// </summary>
        /// <param name="other">The other collider.</param>
        /// <param name="collision">The collision.</param>
        /// <returns>A value indicating whether or not the collision occurred.</returns>
        public bool CollidesWith(Collider other, out CollisionEventArgs collision) {
            if (!this.BoundingArea.Overlaps(other.BoundingArea)) {
                collision = null;
                return false;
            }

            var overlap = float.MaxValue;
            var smallestAxis = Vector2.Zero;

            var axes = this.GetAxesForSAT(other);
            axes.AddRange(other.GetAxesForSAT(this));

            var thisContainsOther = true;
            var otherContainsThis = true;

            foreach (var axis in axes) {
                var projectionA = this.GetProjection(axis);
                var projectionB = other.GetProjection(axis);

                if (projectionA.OverlapsWith(projectionB)) {
                    var currentOverlap = projectionA.GetOverlap(projectionB);

                    var aContainsB = projectionA.Contains(projectionB);
                    var bContainsA = projectionB.Contains(projectionA);

                    // Information gathering for the collision
                    if (!aContainsB) {
                        thisContainsOther = false;
                    }
                    if (!bContainsA) {
                        otherContainsThis = false;
                    }

                    if (aContainsB || bContainsA) {
                        var minimum = Math.Abs(projectionA.Minimum - projectionB.Minimum);
                        var maximum = Math.Abs(projectionA.Maximum - projectionB.Maximum);

                        if (minimum < maximum) {
                            currentOverlap += minimum;
                        }
                        else {
                            currentOverlap += maximum;
                        }
                    }

                    if (currentOverlap < overlap) {
                        overlap = currentOverlap;
                        smallestAxis = axis;
                    }
                }
                else {
                    // Gap was found, return false.
                    collision = null;
                    return false;
                }
            }

            var minimumTranslation = overlap * smallestAxis;

            // This is important to determine which direction this collider has to move to be
            // separated from the other collider.
            if (Vector2.Dot(this.GetCenter() - other.GetCenter(), minimumTranslation) < 0) {
                minimumTranslation = -minimumTranslation;
            }

            collision = new CollisionEventArgs(this, other, smallestAxis, minimumTranslation, thisContainsOther, otherContainsThis);
            return true;
        }

        /// <summary>
        /// Determines whether this collider contains the other collider.
        /// </summary>
        /// <param name="other">The other collider.</param>
        /// <returns><c>true</c> if this collider contains the other collider; otherwise, <c>false</c>.</returns>
        public abstract bool Contains(Collider other);

        /// <summary>
        /// Determines whether this collider contains the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if this collider contains the specified point; otherwise, <c>false</c>.</returns>
        public abstract bool Contains(Vector2 point);

        /// <summary>
        /// Gets the axes to test for the Separating Axis Theorem.
        /// </summary>
        /// <param name="other">The other collider.</param>
        /// <returns>The axes to test for the Separating Axis Theorem</returns>
        public abstract List<Vector2> GetAxesForSAT(Collider other);

        /// <summary>
        /// Gets the center of the collider.
        /// </summary>
        /// <returns>The center of the collider.</returns>
        public abstract Vector2 GetCenter();

        /// <summary>
        /// Gets the projection of this collider onto the specified axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <returns>The projection of this collider onto the specified axis.</returns>
        public abstract Projection GetProjection(Vector2 axis);

        /// <summary>
        /// Determines whether [is candidate for collision] [the specified other].
        /// </summary>
        /// <param name="other">The other collider.</param>
        /// <returns></returns>
        public bool IsCandidateForCollision(Collider other) {
            return this.BoundingArea.Overlaps(other.BoundingArea);
        }

        /// <summary>
        /// Determines whether this collider is hit by the specified ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <param name="hit">The hit.</param>
        /// <returns><c>true</c> if this collider is hit by the specified ray; otherwise, <c>false</c>.</returns>
        public bool IsHitBy(LineSegment ray, out RaycastHit hit) {
            if (!ray.BoundingArea.Overlaps(this.BoundingArea)) {
                hit = null;
                return false;
            }

            return this.TryHit(ray, out hit);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="body">The body.</param>
        internal void Initialize(Body body) {
            this.Body = body;
            this.Reset();
        }

        /// <summary>
        /// Resets components of this collider. This includes the bounding area.
        /// </summary>
        internal virtual void Reset() {
            this._transform = this._transform.Reset(() => this.Body.GetWorldTransform(this.Offset));
            this._boundingArea = this._boundingArea.Reset(this.CreateBoundingArea);
        }

        /// <summary>
        /// Creates the bounding area.
        /// </summary>
        /// <returns>The created bounding area.</returns>
        protected abstract BoundingArea CreateBoundingArea();

        /// <summary>
        /// Gets the normal from the line defined by two points.
        /// </summary>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="secondPoint">The second point.</param>
        /// <returns>The normal.</returns>
        protected Vector2 GetNormal(Vector2 firstPoint, Vector2 secondPoint) {
            var edge = firstPoint - secondPoint;
            var normal = edge.GetPerpendicular();
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// Tries to process a hit between this collider and a ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether or not a hit occurred.</returns>
        protected abstract bool TryHit(LineSegment ray, out RaycastHit result);
    }
}
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Macabre2D.Framework.Physics {

    /// <summary>
    /// A projection used by the physics engine when implementing the Separating Axis Theorem
    /// </summary>
    public struct Projection {

        /// <summary>
        /// The axis on which things are being projected.
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        /// The maximum value on the axis for this projection.
        /// </summary>
        public readonly float Maximum;

        /// <summary>
        /// The minimum value on the axis for this projection.
        /// </summary>
        public readonly float Minimum;

        /// <summary>
        /// Initializes a new instance of the <see cref="Projection"/> struct.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        public Projection(Vector2 axis, float minimum, float maximum) {
            this.Axis = axis;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Creates the polygon's projection on the specified axis.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <param name="axis">The axis.</param>
        /// <returns>The polygon's projection on the specified axis.</returns>
        public static Projection CreatePolygonProjection(Vector2 axis, IEnumerable<Vector2> worldPoints) {
            return CreatePolygonProjection(axis, 0f, worldPoints);
        }

        /// <summary>
        /// Creates the polygon's projection on the specified axis.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The polygon's projection on the specified axis.</returns>
        public static Projection CreatePolygonProjection(Vector2 axis, float offset, IEnumerable<Vector2> worldPoints) {
            var minimum = Vector2.Dot(axis, worldPoints.ElementAt(0));
            var maximum = minimum;

            for (var i = 1; i < worldPoints.Count(); i++) {
                var dotProduct = Vector2.Dot(axis, worldPoints.ElementAt(i));

                if (dotProduct < minimum) {
                    minimum = dotProduct;
                }

                if (dotProduct > maximum) {
                    maximum = dotProduct;
                }
            }

            minimum += offset;
            maximum += offset;

            return new Projection(axis, minimum, maximum);
        }

        /// <summary>
        /// Gets the axis normal.
        /// </summary>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="secondPoint">The second point.</param>
        /// <returns>The axis normal.</returns>
        public static Vector2 GetAxisNormal(Vector2 firstPoint, Vector2 secondPoint) {
            var point = new Vector2(-(secondPoint.Y - firstPoint.Y), secondPoint.X - firstPoint.X);
            point.Normalize();
            return point;
        }

        /// <summary>
        /// Determines whether this projection contains the other projection.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// <c>true</c> if this projection contains the other projection; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Projection other) {
            return this.Maximum > other.Maximum && this.Minimum < other.Minimum;
        }

        /// <summary>
        /// Gets the overlap between the two projections.
        /// </summary>
        /// <param name="other">The other projection.</param>
        /// <returns>The overlap.</returns>
        public float GetOverlap(Projection other) {
            return Math.Max(0f, Math.Min(this.Maximum, other.Maximum) - Math.Max(this.Minimum, other.Minimum));
        }

        /// <summary>
        /// Checks whether this overlaps with the other projection.
        /// </summary>
        /// <param name="other">The other projection.</param>
        /// <returns>A value indicating whether or not this overlaps with the other projection.</returns>
        public bool OverlapsWith(Projection other) {
            return !(this.Maximum <= other.Minimum || other.Maximum <= this.Minimum);
        }
    }
}
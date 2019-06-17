namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Linq;

    /// <summary>
    /// Represents the area around a collider in which a potential collision could be happening. This
    /// is an axis aligned bounding box.
    /// </summary>
    public struct BoundingArea {

        /// <summary>
        /// The height of this instance.
        /// </summary>
        public readonly float Height;

        /// <summary>
        /// The maximum poositions of this instance.
        /// </summary>
        public readonly Vector2 Maximum;

        /// <summary>
        /// The minimum positions of this instance.
        /// </summary>
        public readonly Vector2 Minimum;

        /// <summary>
        /// The width of this instance.
        /// </summary>
        public readonly float Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingArea"/> class.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        public BoundingArea(Vector2 minimum, Vector2 maximum) {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Height = this.Maximum.Y - this.Minimum.Y;
            this.Width = this.Maximum.X - this.Minimum.X;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingArea"/> class.
        /// </summary>
        /// <param name="minimumX">The minimum x.</param>
        /// <param name="maximumX">The maximum x.</param>
        /// <param name="minimumY">The minimum y.</param>
        /// <param name="maximumY">The maximum y.</param>
        public BoundingArea(float minimumX, float maximumX, float minimumY, float maximumY) {
            this.Minimum = new Vector2(minimumX, minimumY);
            this.Maximum = new Vector2(maximumX, maximumY);
            this.Height = maximumY - minimumY;
            this.Width = maximumX - minimumX;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingArea"/> class.
        /// </summary>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        public BoundingArea(float minimumValue, float maximumValue) {
            this.Minimum = new Vector2(minimumValue);
            this.Maximum = new Vector2(maximumValue);
            this.Height = maximumValue - minimumValue;
            this.Width = this.Height;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty {
            get {
                return this.Minimum == this.Maximum;
            }
        }

        /// <summary>
        /// Creates a bounding area from points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The bounding area which confines the provided points.</returns>
        public static BoundingArea CreateFromPoints(params Vector2[] points) {
            return new BoundingArea(
                points.Min(p => p.X),
                points.Max(p => p.X),
                points.Min(p => p.Y),
                points.Max(p => p.Y));
        }

        /// <summary>
        /// Gets a value indicating whether or not this bounding area contains the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c>, if the bounding area contains the point, <c>false</c> otherwise.</returns>
        public bool Contains(Vector2 point) {
            return this.Minimum.X <= point.X && this.Minimum.Y <= point.Y && this.Maximum.X >= point.X && this.Maximum.Y >= point.Y;
        }

        /// <summary>
        /// Gets a value indicating whether or not this bounding area overlaps another specified
        /// bounding area.
        /// </summary>
        /// <param name="other">The other bounding area.</param>
        /// <returns><c>true</c>, if the two bounding areas overlap, <c>false</c> otherwise.</returns>
        public bool Overlaps(BoundingArea other) {
            if (this.Maximum.X < other.Minimum.X || this.Minimum.X > other.Maximum.X) {
                return false;
            }
            else if (this.Maximum.Y < other.Minimum.Y || this.Minimum.Y > other.Maximum.Y) {
                return false;
            }

            return true;
        }
    }
}
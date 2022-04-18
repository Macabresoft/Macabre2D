namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents the area around a collider in which a potential collision could be happening.
/// This is an axis aligned bounding box.
/// </summary>
[DataContract]
public readonly struct BoundingArea {
    /// <summary>
    /// The empty bounding area.
    /// </summary>
    public static readonly BoundingArea Empty = new(0f, 0f);

    /// <summary>
    /// The maximum sized bounding area.
    /// </summary>
    public static readonly BoundingArea MaximumSize = new(float.MinValue * 0.5f, float.MaxValue * 0.5f);

    /// <summary>
    /// The height of this instance.
    /// </summary>
    public float Height => this.Maximum.Y - this.Minimum.Y;

    /// <summary>
    /// The maximum positions of this instance.
    /// </summary>
    [DataMember]
    public readonly Vector2 Maximum;

    /// <summary>
    /// The minimum positions of this instance.
    /// </summary>
    [DataMember]
    public readonly Vector2 Minimum;

    /// <summary>
    /// The width of this instance.
    /// </summary>
    public readonly float Width => this.Maximum.X - this.Minimum.X;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public BoundingArea(Vector2 minimum, Vector2 maximum) {
        this.Minimum = minimum;
        this.Maximum = maximum;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public BoundingArea(Vector2 minimum, float width, float height) : this(minimum, new Vector2(minimum.X + width, minimum.Y + height)) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
    /// </summary>
    /// <param name="minimumX">The minimum x.</param>
    /// <param name="maximumX">The maximum x.</param>
    /// <param name="minimumY">The minimum y.</param>
    /// <param name="maximumY">The maximum y.</param>
    public BoundingArea(float minimumX, float maximumX, float minimumY, float maximumY) : this(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY)) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
    /// </summary>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    public BoundingArea(float minimumValue, float maximumValue) : this(new Vector2(minimumValue), new Vector2(maximumValue)) {
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
    public bool IsEmpty => this.Minimum == this.Maximum;

    /// <summary>
    /// Combines the specified bounding areas.
    /// </summary>
    /// <param name="boundingAreas">The bounding areas.</param>
    /// <returns>A bounding area that includes all provided bounding areas.</returns>
    public static BoundingArea Combine(params BoundingArea[] boundingAreas) {
        if (boundingAreas.Any()) {
            var minimums = boundingAreas.Select(x => x.Minimum).ToArray();
            var maximums = boundingAreas.Select(x => x.Maximum).ToArray();
            return new BoundingArea(minimums.Select(x => x.X).Min(), maximums.Select(x => x.X).Max(), minimums.Select(x => x.Y).Min(), maximums.Select(x => x.Y).Max());
        }

        return Empty;
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
    /// <returns><c>true</c>, if this bounding area contains the point, <c>false</c> otherwise.</returns>
    public bool Contains(Vector2 point) {
        var (x, y) = point;
        return (this.Minimum.X <= x || this.Minimum.X.HasMinimalDifference(x)) &&
               (this.Minimum.Y <= y || this.Minimum.Y.HasMinimalDifference(y)) &&
               (this.Maximum.X >= x || this.Maximum.X.HasMinimalDifference(x)) &&
               (this.Maximum.Y >= y || this.Maximum.Y.HasMinimalDifference(y));
    }

    /// <summary>
    /// Gets a value indicating whether or not this bounding area contains the specified bounding area within its bounds.
    /// </summary>
    /// <param name="other">The other bounding area.</param>
    /// <returns><c>true</c>, if this bounding area contains the other bounding area, <c>false</c> otherwise.</returns>
    public bool Contains(BoundingArea other) {
        return (this.Minimum.X <= other.Minimum.X || this.Minimum.X.HasMinimalDifference(other.Minimum.X)) &&
               (this.Minimum.Y <= other.Minimum.Y || this.Minimum.Y.HasMinimalDifference(other.Minimum.Y)) &&
               (this.Maximum.X >= other.Maximum.X || this.Maximum.X.HasMinimalDifference(other.Maximum.X)) &&
               (this.Maximum.Y >= other.Maximum.Y || this.Maximum.Y.HasMinimalDifference(other.Maximum.Y));
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

        if (this.Maximum.Y < other.Minimum.Y || this.Minimum.Y > other.Maximum.Y) {
            return false;
        }

        return true;
    }
}
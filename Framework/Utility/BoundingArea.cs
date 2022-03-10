namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents the area around a collider in which a potential collision could be happening.
/// This is an axis aligned bounding box.
/// </summary>
public struct BoundingArea {
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
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
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
    /// <returns><c>true</c>, if the bounding area contains the point, <c>false</c> otherwise.</returns>
    public bool Contains(Vector2 point) {
        return (this.Minimum.X <= point.X || this.Minimum.X.HasMinimalDifference(point.X)) &&
               (this.Minimum.Y <= point.Y || this.Minimum.Y.HasMinimalDifference(point.Y)) &&
               (this.Maximum.X >= point.X || this.Maximum.X.HasMinimalDifference(point.X)) &&
               (this.Maximum.Y >= point.Y || this.Maximum.Y.HasMinimalDifference(point.Y));
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
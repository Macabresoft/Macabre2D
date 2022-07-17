namespace Macabresoft.Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;

/// <summary>
/// Represents a ray in the physics system.
/// </summary>
public sealed class LineSegment : IBoundable {
    public static readonly LineSegment Empty = new(Vector2.Zero, Vector2.Zero, Vector2.Zero, 0f);

    /// <summary>
    /// The direction of the ray.
    /// </summary>
    public readonly Vector2 Direction;

    /// <summary>
    /// The distance the ray will travel (we do not allow infinite rays).
    /// </summary>
    public readonly float Distance;

    /// <summary>
    /// The end point of the ray.
    /// </summary>
    public readonly Vector2 End;

    /// <summary>
    /// The starting point of the ray.
    /// </summary>
    public readonly Vector2 Start;

    private readonly Lazy<BoundingArea> _boundingArea;
    private readonly Lazy<float> _valueA;
    private readonly Lazy<float> _valueB;
    private readonly Lazy<float> _valueC;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment" /> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public LineSegment(Vector2 start, Vector2 end) : this(start, end, (end - start).GetNormalized(), (end - start).Length()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment" /> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="distance">The distance.</param>
    public LineSegment(Vector2 start, Vector2 direction, float distance) : this(start, start + direction * distance, direction, distance) {
    }

    private LineSegment(Vector2 start, Vector2 end, Vector2 direction, float distance) {
        this.Start = start;
        this.End = end;
        this.Direction = direction;
        this.Distance = distance;
        this._boundingArea = new Lazy<BoundingArea>(() => BoundingArea.CreateFromPoints(this.Start, this.End));
        this._valueA = new Lazy<float>(() => this.End.Y - this.Start.Y);
        this._valueB = new Lazy<float>(() => this.Start.X - this.End.X);
        this._valueC = new Lazy<float>(() => this.ValueA * this.Start.X + this.ValueB * this.Start.Y);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    internal float ValueA => this._valueA.Value;

    internal float ValueB => this._valueB.Value;

    internal float ValueC => this._valueC.Value;

    /// <summary>
    /// Gets a value indicating whether or not this line segment contains a specific point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>A value indicating whether or not this line segment contains a specific point.</returns>
    public bool Contains(Vector2 point) {
        if (this.Distance == 0f) {
            return false;
        }

        if (point == this.Start || point == this.End) {
            return true;
        }

        if (this.BoundingArea.Contains(point)) {
            var pointCrossX = point.X - this.Start.X;
            var pointCrossY = point.Y - this.Start.Y;
            var lineCrossX = this.End.X - this.Start.X;
            var lineCrossY = this.End.Y - this.Start.Y;
            var cross = pointCrossX * lineCrossY - pointCrossY * lineCrossX;

            if (cross == 0f) {
                // The point is on this line if it were infinitely long, but now make sure it is on this particular segment.
                if (Math.Abs(lineCrossX) >= Math.Abs(lineCrossY)) {
                    return lineCrossX > 0f ? this.Start.X <= point.X && point.X <= this.End.X : this.End.X <= point.X && point.X <= this.Start.X;
                }

                return lineCrossY > 0f ? this.Start.Y <= point.Y && point.Y <= this.End.Y : this.End.Y <= point.Y && point.Y <= this.Start.Y;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the center of this line segment.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCenter() {
        var x = 0.5f * (this.Start.X + this.End.X);
        var y = 0.5f * (this.Start.Y + this.End.Y);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Gets the perpendicular normal of this line segment.
    /// </summary>
    /// <returns>The perpendicular normal of this line segment</returns>
    public Vector2 GetNormal() {
        var edge = this.Start - this.End;
        var normal = new Vector2(-edge.Y, edge.X);
        normal.Normalize();
        return normal;
    }

    /// <summary>
    /// Gets a value indicating whether or not this line segment intersects with the other line segment.
    /// </summary>
    /// <param name="other">The other line segment.</param>
    /// <param name="intersection">The intersection.</param>
    /// <returns>
    /// A value indicating whether or not this line segment intersects with the other line segment.
    /// </returns>
    public bool Intersects(LineSegment other, out Vector2 intersection) {
        var det = this.ValueA * other.ValueB - other.ValueA * this.ValueB;
        if (det == 0f) {
            intersection = Vector2.Zero;
            return false;
        }

        var x = (other.ValueB * this.ValueC - this.ValueB * other.ValueC) / det;
        var y = (this.ValueA * other.ValueC - other.ValueA * this.ValueC) / det;
        intersection = new Vector2(x, y);

        if (this.BoundingArea.Contains(intersection) && other.BoundingArea.Contains(intersection)) {
            return true;
        }

        return false;
    }
}
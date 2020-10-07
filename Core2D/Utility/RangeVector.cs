namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents the maximum and minimum of two floating point numbers.
    /// </summary>
    public struct RangeVector {

        /// <summary>
        /// The length of the range.
        /// </summary>
        public float Length;

        /// <summary>
        /// The maximum value in the range.
        /// </summary>
        public float Max;

        /// <summary>
        /// The minimum value in the range.
        /// </summary>
        public float Min;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeVector" /> struct.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public RangeVector(Vector2 vector) : this(vector.X, vector.Y) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeVector" /> struct.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        public RangeVector(float value1, float value2) {
            var length = value1 - value2;

            if (length < 0) {
                this.Min = value1;
                this.Max = value2;
                this.Length = -length;
            }
            else {
                this.Max = value1;
                this.Min = value2;
                this.Length = length;
            }
        }

        /// <inheritdoc />
        public static bool operator !=(RangeVector left, RangeVector right) {
            return !(left == right);
        }

        /// <inheritdoc />
        public static bool operator ==(RangeVector left, RangeVector right) {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) {
            var result = true;
            if (obj is RangeVector range) {
                result = range.Min == this.Min && range.Max == this.Max;
            }

            return result;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return System.HashCode.Combine(this.Max, this.Min);
        }

        /// <summary>
        /// Converts to <see cref="Vector2" /> with <see cref="Vector2.X" /> as the minimum value
        /// and <see cref="Vector2.Y" /> as the maximum value.
        /// </summary>
        /// <returns>A vector.</returns>
        public Vector2 ToVector2() {
            return new Vector2(this.Min, this.Max);
        }
    }
}
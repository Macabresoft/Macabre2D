namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Extension methods to handle math.
    /// </summary>
    public static class MathExtensions {

        /// <summary>
        /// Clamps a <see cref="ushort"/> betweem a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minumum">The minumum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>The clamped value.</returns>
        public static ushort Clamp(this ushort value, ushort minumum, ushort maximum) {
            return Math.Max(minumum, Math.Min(maximum, value));
        }

        /// <summary>
        /// Clamps a <see cref="float"/> between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(this float value, float minimum, float maximum) {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        /// <summary>
        /// Determines whether the value is a power of two.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is a power of two; otherwise, <c>false</c>.</returns>
        public static bool IsPowerOfTwo(this int value) {
            return (value != 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Normalizes an angle to the radian unit circle.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The normalized angle.</returns>
        public static float NormalizeAngle(this float value) {
            while (value >= MathHelper.TwoPi) {
                value -= MathHelper.TwoPi;
            }

            while (value < 0) {
                value += MathHelper.TwoPi;
            }

            return value;
        }

        /// <summary>
        /// Converts a value to negative one or one.
        /// </summary>
        /// <remarks>Zero is treated as positive in this case.</remarks>
        /// <param name="value">The value.</param>
        /// <returns>The sign.</returns>
        public static float ToSign(this float value) {
            return value < 0 ? -1f : 1f;
        }
    }
}
namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Extension methods for dealing with pixel snapping.
    /// </summary>
    public static class PixelSnapExtensions {

        /// <summary>
        /// Converts a <see cref="float"/> to a pixel snapped value according to the <see cref="IGameSettings"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A pixel snapped value</returns>
        public static float ToPixelSnappedValue(this float value) {
            return (int)Math.Round(value * GameSettings.Instance.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * GameSettings.Instance.InversePixelsPerUnit;
        }

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a pixel snapped value according to the <see cref="IGameSettings"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A pixel snapped value</returns>
        public static Vector2 ToPixelSnappedValue(this Vector2 value) {
            return new Vector2(
                (int)Math.Round(value.X * GameSettings.Instance.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * GameSettings.Instance.InversePixelsPerUnit,
                (int)Math.Round(value.Y * GameSettings.Instance.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * GameSettings.Instance.InversePixelsPerUnit);
        }

        /// <summary>
        /// Converts a <see cref="Transform"/> to a pixel snapped value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>A pixel snapped value.</returns>
        public static Transform ToPixelSnappedValue(this Transform transform) {
            var position = transform.Position.ToPixelSnappedValue();
            var scale = new Vector2((int)Math.Round(transform.Scale.X, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.Scale.Y, 0, MidpointRounding.AwayFromZero));
            return new Transform(position, scale);
        }
    }
}
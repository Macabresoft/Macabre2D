namespace Macabresoft.Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for dealing with pixel snapping.
/// </summary>
public static class PixelSnapExtensions {
    /// <summary>
    /// Gets a value indicating whether or not the entity should snap to pixels.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="project">The project.</param>
    /// <returns>A value indicating whether or not this should snap to pixels.</returns>
    public static bool ShouldSnapToPixels(this IPixelSnappable entity, IGameProject project) {
        return entity.PixelSnap == PixelSnap.Yes || (entity.PixelSnap == PixelSnap.Inherit && project.SnapToPixels);
    }

    /// <summary>
    /// Converts a <see cref="float" /> to a pixel snapped value according to the <see cref="IGameProject" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="project">The project.</param>
    /// <returns>A pixel snapped value</returns>
    public static float ToPixelSnappedValue(this float value, IGameProject project) {
        return (int)Math.Round(value * project.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * project.UnitsPerPixel;
    }

    /// <summary>
    /// Converts a <see cref="Vector2" /> to a pixel snapped value according to the <see cref="IGameProject" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="project">The project.</param>
    /// <returns>A pixel snapped value</returns>
    public static Vector2 ToPixelSnappedValue(this Vector2 value, IGameProject project) {
        return value.ToPixelSnappedValue(project, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Converts a <see cref="Vector2" /> to a pixel snapped value according to the <see cref="IGameProject" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="project">The project.</param>
    /// <param name="midpointRounding">The midpoint rounding.</param>
    /// <returns>A pixel snapped value</returns>
    public static Vector2 ToPixelSnappedValue(this Vector2 value, IGameProject project, MidpointRounding midpointRounding) {
        return new Vector2(
            (int)Math.Round(value.X * project.PixelsPerUnit, 0, midpointRounding) * project.UnitsPerPixel,
            (int)Math.Round(value.Y * project.PixelsPerUnit, 0, midpointRounding) * project.UnitsPerPixel);
    }

    /// <summary>
    /// Converts a <see cref="BoundingArea" /> to a pixel snapped value according to the <see cref="IGameProject" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="project">The project.</param>
    /// <returns>A pixel snapped value.</returns>
    public static BoundingArea ToPixelSnappedValue(this BoundingArea value, IGameProject project) {
        return new BoundingArea(
            value.Minimum.ToPixelSnappedValue(project, MidpointRounding.ToNegativeInfinity),
            value.Maximum.ToPixelSnappedValue(project, MidpointRounding.ToPositiveInfinity));
    }
}
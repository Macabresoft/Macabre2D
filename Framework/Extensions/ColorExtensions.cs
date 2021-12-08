namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for <see cref="Color" />.
/// </summary>
public static class ColorExtensions {
    /// <summary>
    /// Gets either black or white depending on which contrasts with the provided color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>Black or white depending on which contrasts with the provided color.</returns>
    public static Color GetContrastingBlackOrWhite(this Color color) {
        var luma = color.GetLuma();
        return luma < 127 ? Color.White : Color.Black;
    }

    /// <summary>
    /// Gets the luma value for the provided color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The luma value.</returns>
    public static float GetLuma(this Color color) {
        color.Deconstruct(out var red, out var green, out byte blue);
        return 0.2126f * red + 0.7152f * green + 0.0722f * blue;
    }
}
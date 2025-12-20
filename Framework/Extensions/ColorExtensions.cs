namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// Extension methods for <see cref="Color" />.
/// </summary>
public static class ColorExtensions {
    /// <param name="color">The color.</param>
    extension(Color color) {
        /// <summary>
        /// Gets either black or white depending on which contrasts with the provided color.
        /// </summary>
        /// <returns>Black or white depending on which contrasts with the provided color.</returns>
        public Color GetContrastingBlackOrWhite() {
            var luma = color.GetLuma();
            return luma < 127 ? Color.White : Color.Black;
        }

        /// <summary>
        /// Gets the luma value for the provided color.
        /// </summary>
        /// <returns>The luma value.</returns>
        public float GetLuma() {
            color.Deconstruct(out var red, out var green, out byte blue);
            return 0.2126f * red + 0.7152f * green + 0.0722f * blue;
        }

        /// <summary>
        /// Converts the color to a hex code.
        /// </summary>
        /// <returns>The hex code.</returns>
        public string ToHex() => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
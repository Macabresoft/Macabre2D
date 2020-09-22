namespace Macabresoft.MonoGame.AvaloniaUI.Extensions {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Extensions for <see cref="Color" />.
    /// </summary>
    public static class ColorExtensions {

        /// <summary>
        /// Converts from a MonoGame color to an Avalonia brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The Avalonia brush.</returns>
        public static Avalonia.Media.IBrush ToAvaloniaBrush(this Color color) {
            return new Avalonia.Media.SolidColorBrush(color.ToAvaloniaColor(), 1d);
        }

        /// <summary>
        /// Converts from a MonoGame color to an Avalonia color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The color in Avalonia's format.</returns>
        public static Avalonia.Media.Color ToAvaloniaColor(this Color color) {
            return new Avalonia.Media.Color(color.A, color.R, color.G, color.B);
        }
    }
}
namespace Macabre2D.UI.AvaloniaInterop;

using Avalonia.Media;
using Color = Microsoft.Xna.Framework.Color;

/// <summary>
/// Extensions for <see cref="Microsoft.Xna.Framework.Color" />.
/// </summary>
public static class ColorExtensions {
    /// <summary>
    /// Converts from a MonoGame color to an Avalonia brush.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The Avalonia brush.</returns>
    public static IBrush ToAvaloniaBrush(this Color color) {
        return new SolidColorBrush(color.ToAvaloniaColor());
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
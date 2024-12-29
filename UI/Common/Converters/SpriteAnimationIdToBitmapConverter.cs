namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// Takes a sprite animation identifier and a <see cref="SpriteDisplayCollection" />
/// </summary>
public class SpriteAnimationIdToBitmapConverter : IMultiValueConverter {

    /// <summary>
    /// Gets the static instance of <see cref="SpriteAnimationIdToBitmapConverter" />.
    /// </summary>
    public static SpriteAnimationIdToBitmapConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        if (values.OfType<Guid>().FirstOrDefault() is var id &&
            id != Guid.Empty &&
            values.OfType<SpriteDisplayCollection>().FirstOrDefault() is { } collection) {
            return collection.Sprites.FirstOrDefault(x => x.Member?.Id == id)?.Bitmap;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

/// <summary>
/// Takes a sprite index and a <see cref="SpriteDisplayCollection" />
/// </summary>
public class SpriteIndexToBitmapConverter : IMultiValueConverter {
    /// <inheritdoc />
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        if (values.OfType<SpriteDisplayCollection>().FirstOrDefault() is { } collection &&
            values.OfType<byte?>().FirstOrDefault() is { } index) {
            return collection.Sprites.FirstOrDefault(x => x.Index == index)?.Bitmap;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
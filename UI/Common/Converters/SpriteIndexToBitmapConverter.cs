namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Avalonia;
    using Avalonia.Data.Converters;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;

    /// <summary>
    /// Takes a sprite index and a <see cref="SpriteDisplayCollection" />
    /// </summary>
    public class SpriteIndexToBitmapConverter : IMultiValueConverter {
        /// <inheritdoc />
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
            if (values.OfType<SpriteDisplayCollection>().FirstOrDefault() is SpriteDisplayCollection collection &&
                values.OfType<byte>().FirstOrDefault() is var index and > 0) {
                return collection.Sprites.FirstOrDefault(x => x.Index == index);
            }

            return AvaloniaProperty.UnsetValue;
        }
    }
}
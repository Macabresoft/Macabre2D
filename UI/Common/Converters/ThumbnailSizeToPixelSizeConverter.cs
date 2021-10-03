namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Data.Converters;

    public class ThumbnailSizeToPixelSizeConverter : IValueConverter {
        private const double Large = Medium * 2d;
        private const double Medium = Small * 2d;
        private const double Small = 64d;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is ThumbnailSize size) {
                return size switch {
                    ThumbnailSize.Small => Small,
                    ThumbnailSize.Medium => Medium,
                    ThumbnailSize.Large => Large,
                    _ => AvaloniaProperty.UnsetValue
                };
            }

            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is double doubleValue) {
                return doubleValue switch {
                    Small => ThumbnailSize.Small,
                    Medium => ThumbnailSize.Medium,
                    Large => ThumbnailSize.Large,
                    _ => AvaloniaProperty.UnsetValue
                };
            }

            return AvaloniaProperty.UnsetValue;
        }
    }
}
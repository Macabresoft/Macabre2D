namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Macabresoft.Core;

    /// <summary>
    /// Converts from a <see cref="Type" /> or <see cref="Enum" /> to a display name.
    /// </summary>
    public class ToDisplayNameConverter : IValueConverter {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = string.Empty;
            if (value is Enum enumValue) {
                result = enumValue.GetEnumDisplayName();
            }
            else if (value is Type type) {
                result = type.GetTypeDisplayName();
            }
            else if (value != null) {
                result = value.ToString();
            }

            return result;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Converts from a <see cref="Type" /> or <see cref="Enum" /> to a display name.
    /// </summary>
    public class ToDisplayNameConverter : IValueConverter {

        /// <summary>
        /// A static instance of this converter.
        /// </summary>
        public static readonly ToDisplayNameConverter Instance = new();
        
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value switch {
                Enum enumValue => enumValue.GetEnumDisplayName(),
                Type type => type.GetTypeDisplayName(),
                INameable nameable => nameable.Name,
                _ => value?.GetType().GetTypeDisplayName() ?? string.Empty
            };
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
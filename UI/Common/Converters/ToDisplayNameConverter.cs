namespace Macabresoft.Macabre2D.UI.Common.Converters {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
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
                Enum enumValue => GetEnumName(enumValue),
                Type type => type.GetTypeDisplayName(),
                INameable nameable => nameable.Name,
                _ => value?.GetType().GetTypeDisplayName() ?? string.Empty
            };
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        private static string GetEnumName(Enum enumValue) {
            var enumType = enumValue.GetType();
            if (enumType.GetCustomAttribute<FlagsAttribute>() != null) {
                var displayName = string.Empty;
                foreach (var value in Enum.GetValues(enumType).OfType<Enum>().Where(x => enumValue.HasFlag(x))) {
                    var valueDisplayName = value.GetEnumDisplayName();
                    if (!string.IsNullOrEmpty(valueDisplayName)) {
                        displayName = string.IsNullOrEmpty(displayName) ?
                            value.GetEnumDisplayName() :
                            $"{displayName}, {value.GetEnumDisplayName()}";
                    }

                }

                return displayName;
            }
            else {
                return enumValue.GetEnumDisplayName();
            }
        }
    }
}
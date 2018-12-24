namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    public sealed class AssetTypeMaskToVisibilityConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;
            var actualValues = values.OfType<AssetType>().ToList();

            if (actualValues.Count == 2 && (actualValues[0] == AssetType.Folder || (actualValues[0] & actualValues[1]) == actualValues[0])) {
                result = Visibility.Visible;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
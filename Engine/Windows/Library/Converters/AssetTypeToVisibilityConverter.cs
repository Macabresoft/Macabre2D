namespace Macabre2D.Engine.Windows.Converters {

    using Macabre2D.Engine.Windows.Models;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class AssetTypeToVisibilityConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var result = Visibility.Collapsed;

            if (values.Length == 2 && values[0] is Asset asset && values[1] is Type type && (typeof(FolderAsset).IsAssignableFrom(asset.GetType()) || type.IsAssignableFrom(asset.GetType()))) {
                result = Visibility.Visible;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
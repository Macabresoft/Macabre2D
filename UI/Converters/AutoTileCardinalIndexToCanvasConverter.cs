namespace Macabre2D.UI.Converters {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class AutoTileCardinalIndexToCanvasConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = DependencyProperty.UnsetValue;
            if (value is int index) {
                result = Application.Current.TryFindResource($"AutoTileCanvasCardinal{index}");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
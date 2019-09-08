namespace Macabre2D.UI.Converters {

    using Macabre2D.Framework;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class LayerToNameConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = value?.ToString();

            if (value is Layers layer || Enum.TryParse(value.ToString(), out layer)) {
                result = GameSettings.Instance.GetLayerName(layer);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
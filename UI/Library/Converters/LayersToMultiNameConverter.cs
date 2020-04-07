namespace Macabre2D.UI.Library.Converters {

    using Macabre2D.Framework;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public sealed class LayersToMultiNameConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = value?.ToString();
            if (value is Layers layers) {
                result = string.Empty;
                var layerList = layers.ToString().Split(',').Select(flag => (Layers)Enum.Parse(typeof(Layers), flag)).ToList();
                foreach (var layer in layerList) {
                    if (!string.IsNullOrEmpty(result)) {
                        result = $"{result}, {GameSettings.Instance.Layers.GetLayerName(layer)}";
                    }
                    else {
                        result = GameSettings.Instance.Layers.GetLayerName(layer);
                    }
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
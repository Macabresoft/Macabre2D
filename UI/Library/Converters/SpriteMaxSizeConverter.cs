namespace Macabre2D.UI.Library.Converters {

    using Microsoft.Xna.Framework;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class SpriteMaxSizeConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var result = 0;
            if (values.Length > 1 && values[0] is int imageSize && values[1] is Point spriteSize) {
                var sizeForCalculation = parameter?.ToString()?.ToUpper() == "X" ? spriteSize.X : spriteSize.Y;
                result = imageSize - sizeForCalculation;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
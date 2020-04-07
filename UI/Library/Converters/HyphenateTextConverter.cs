namespace Macabre2D.UI.Library.Converters {

    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    public sealed class HyphenateTextConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var stringBuilder = new StringBuilder();
            foreach (var value in values) {
                if (value is string text && !string.IsNullOrWhiteSpace(text)) {
                    stringBuilder.Append(text);
                    stringBuilder.Append(" - ");
                }
            }

            if (stringBuilder.Length > 3) {
                stringBuilder.Remove(stringBuilder.Length - 3, 3);
            }

            return stringBuilder.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
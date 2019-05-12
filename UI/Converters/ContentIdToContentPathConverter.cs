namespace Macabre2D.UI.Converters {

    using Macabre2D.Framework;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class ContentIdToContentPathConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string result = null;
            if (value is Guid guid) {
                result = AssetManager.Instance.GetPath(guid);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Macabre2D.UI.CommonLibrary.Converters {

    public sealed class RemoveFileExtensionConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = value;
            if (value is string fileName) {
                result = Path.GetFileNameWithoutExtension(fileName);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
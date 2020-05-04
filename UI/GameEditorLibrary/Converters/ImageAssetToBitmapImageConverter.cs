namespace Macabre2D.UI.GameEditorLibrary.Converters {

    using Macabre2D.UI.GameEditorLibrary.Models;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public sealed class ImageAssetToBitmapImageConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            BitmapImage source = null;
            if (value is ImageAsset imageAsset) {
                var path = imageAsset.GetPath();
                source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(path, UriKind.Absolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
            }

            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
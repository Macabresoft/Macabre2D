namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public sealed class SpriteWrapperToCroppedBitmapConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            CroppedBitmap bitmap = null;
            if (value is SpriteWrapper spriteWrapper) {
                var path = spriteWrapper.GetPath();
                if (!string.IsNullOrWhiteSpace(path)) {
                    var source = new BitmapImage(new Uri(spriteWrapper.GetPath()));

                    var x = Math.Min(source.PixelWidth, spriteWrapper.Location.X);
                    var y = Math.Min(source.PixelHeight, spriteWrapper.Location.Y);
                    var width = Math.Min(source.PixelWidth - x, spriteWrapper.Size.X);
                    var height = Math.Min(source.PixelHeight - y, spriteWrapper.Size.Y);

                    var rect = new System.Windows.Int32Rect(x, y, width, height);
                    bitmap = new CroppedBitmap(source, rect);
                }
            }

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
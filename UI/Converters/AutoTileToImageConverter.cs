namespace Macabre2D.UI.Converters {

    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public sealed class AutoTileToImageConverter : IMultiValueConverter {
        ////public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        ////    if (value is IndexedWrapper<SpriteWrapper> indexedWrapper) {
        ////        if (indexedWrapper.WrappedObject != null) {
        ////            bitmap = SpriteWrapperToCroppedBitmapConverter.ConvertToCroppedBitmap(indexedWrapper.WrappedObject);
        ////        }
        ////        else if (parameter is bool usesIntermediateDirections) {
        ////            if (usesIntermediateDirections) {
        ////            }
        ////            else {
        ////                if (Application.Current.TryFindResource($"AutoTileSetCardinal{indexedWrapper.Index}") is BitmapImage image) {
        ////                    bitmap = new CroppedBitmap(image, image.SourceRect);
        ////                }
        ////            }
        ////        }
        ////    }

        ////}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            CroppedBitmap bitmap = null;

            if (values.Length == 3) {
                if (values[0] is SpriteWrapper spriteWrapper) {
                    bitmap = SpriteWrapperToCroppedBitmapConverter.ConvertToCroppedBitmap(spriteWrapper);
                }
                else if (values[1] is int index && values[2] is bool usesIntermediateDirections) {
                    if (usesIntermediateDirections) {
                    }
                    else {
                        if (Application.Current.TryFindResource($"AutoTileSetCardinal{index}") is BitmapImage image) {
                            bitmap = new CroppedBitmap(image, image.SourceRect);
                        }
                    }
                }
            }

            return bitmap;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
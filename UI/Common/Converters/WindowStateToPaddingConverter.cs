namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data.Converters;
    using Avalonia.Platform;

    public class WindowStateToPaddingConverter : IValueConverter {
        public static readonly Thickness MatchingPadding = new(8);
        public static readonly Thickness UnmatchingPadding = new(0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var operatingSystem = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;
            if (operatingSystem == OperatingSystemType.WinNT && value is WindowState and (WindowState.Maximized or WindowState.FullScreen)) {
                return MatchingPadding;
            }

            return UnmatchingPadding;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data.Converters;
    using Avalonia.Platform;

    /// <summary>
    /// Converts from a <see cref="WindowState" /> to a thickness that accounts for Windows's maximization behavior.
    /// </summary>
    public class WindowStateToContentMarginConverter : IValueConverter {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly WindowStateToContentMarginConverter Instance = new();

        private static readonly Thickness EmptyThickness = new(0d);
        private static readonly Thickness MaximizedThickness = new(6d);

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is WindowState.Maximized && AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT) {
                return MaximizedThickness;
            }
            
            return EmptyThickness;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is Thickness thickness && thickness == MaximizedThickness ? WindowState.Maximized : WindowState.Normal;
        }
    }
}
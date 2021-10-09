namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using Avalonia;
    using Avalonia.Platform;

    /// <summary>
    /// Helper class for windows.
    /// </summary>
    public static class WindowHelper {
        private static readonly Lazy<bool> LazyShowNonNativeMenu = new(
            () => AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT);
        
        /// <summary>
        /// Gets a value indicating whether or not the non-native menu should be shown. The native menu is for MacOS only.
        /// </summary>
        public static bool ShowNonNativeMenu => LazyShowNonNativeMenu.Value;
    }
}
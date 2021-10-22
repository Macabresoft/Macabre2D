namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform;
    using ReactiveUI;

    /// <summary>
    /// Helper class for windows.
    /// </summary>
    public static class WindowHelper {
        private static readonly Lazy<bool> LazyShowNonNativeMenu = new(
            () => AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT);

        /// <summary>
        /// Gets a command to close a dialog.
        /// </summary>
        public static ICommand CloseDialogCommand { get; } = ReactiveCommand.Create<IWindow>(Close);

        /// <summary>
        /// Gets a command to minimize a window.
        /// </summary>
        public static ICommand MinimizeCommand { get; } = ReactiveCommand.Create<IWindow>(Minimize);

        /// <summary>
        /// Gets a value indicating whether or not the non-native menu should be shown. The native menu is for MacOS only.
        /// </summary>
        public static bool ShowNonNativeMenu => LazyShowNonNativeMenu.Value;

        /// <summary>
        /// Gets a command to toggle a window's state.
        /// </summary>
        public static ICommand ToggleWindowStateCommand { get; } = ReactiveCommand.Create<IWindow>(ToggleWindowState);

        private static void Close(IWindow window) {
            window.Close(false);
        }

        private static void Minimize(IWindow window) {
            window.WindowState = WindowState.Minimized;
        }

        private static void ToggleWindowState(IWindow window) {
            window.WindowState = window.WindowState is WindowState.Maximized or WindowState.FullScreen ? WindowState.Normal : WindowState.Maximized;
        }
    }
}
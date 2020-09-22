namespace Macabresoft.MonoGame.AvaloniaUI {

    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.VisualTree;

    internal static class WindowHelper {

        public static bool IsControlOnActiveWindow(IInputElement element) {
            var result = false;
            if (element.GetVisualRoot() is Window window) {
                result = window.IsActive;
            }

            return result;
        }
    }
}
namespace Macabre2D.UI.AvaloniaInterop;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

internal static class ActiveWindowHelper {
    public static bool IsControlOnActiveWindow(InputElement element) {
        var result = false;
        if (element.GetVisualRoot() is Window window) {
            result = window.IsActive;
        }

        return result;
    }
}
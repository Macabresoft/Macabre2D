namespace Macabresoft.AvaloniaEx;

using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

/// <summary>
/// Helper class for windows.
/// </summary>
public static class WindowHelper {
    /// <summary>
    /// Gets a command to close a dialog.
    /// </summary>
    public static ICommand CloseDialogCommand { get; } = ReactiveCommand.Create<IWindow>(Close);

    /// <summary>
    /// Gets a command to minimize a window.
    /// </summary>
    public static ICommand MinimizeCommand { get; } = ReactiveCommand.Create<IWindow>(Minimize);

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
namespace Macabresoft.AvaloniaEx;

using System.ComponentModel;
using Avalonia.Controls;

/// <summary>
/// Interface for <see cref="Window" />.
/// </summary>
public interface IWindow : INotifyPropertyChanged {
    /// <summary>
    /// Gets a value indicating whether or not this can be resized.
    /// </summary>
    bool CanResize { get; }

    /// <summary>
    /// Gets a value indicating whether or not the minimize button should be shown.
    /// </summary>
    bool ShowMinimize { get; }

    /// <summary>
    /// Gets or sets the state of the window.
    /// </summary>
    WindowState WindowState { get; set; }

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the window with the specified result.
    /// </summary>
    /// <param name="dialogResult">The dialog result.</param>
    void Close(object dialogResult);
}
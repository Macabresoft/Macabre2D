namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia.Controls;

    /// <summary>
    /// Interface for <see cref="Window" />, but actually only used by <see cref="MainWindow" /> and dialogs.
    /// </summary>
    public interface IWindow {
        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        WindowState WindowState { get; set; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
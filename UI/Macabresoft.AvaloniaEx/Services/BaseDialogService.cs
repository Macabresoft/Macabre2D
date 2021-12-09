namespace Macabresoft.AvaloniaEx;

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Unity;

/// <summary>
/// An interface for a base dialog service.
/// </summary>
public interface IBaseDialogService {
    /// <summary>
    /// Shows a warning dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <returns>A task.</returns>
    Task ShowWarningDialog(string title, string message);

    /// <summary>
    /// Shows a dialog with a yes and a no button.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message, usually a yes or no question.</param>
    /// <param name="allowCancel">A value indicating whether or not to allow cancellation.</param>
    /// <returns><c>true</c> if the user selected yes; otherwise <c>false</c>.</returns>
    Task<YesNoCancelResult> ShowYesNoDialog(string title, string message, bool allowCancel);
}

/// <summary>
/// A base dialog service.
/// </summary>
public class BaseDialogService : IBaseDialogService {
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDialogService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="mainWindow">The main window.</param>
    protected BaseDialogService(IUnityContainer container, Window mainWindow) {
        this.MainWindow = mainWindow;
        this.Container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <summary>
    /// Gets the unity container.
    /// </summary>
    protected IUnityContainer Container { get; }

    /// <summary>
    /// Gets the main window.
    /// </summary>
    protected Window MainWindow { get; }

    /// <inheritdoc />
    public async Task ShowWarningDialog(string title, string message) {
        var window = this.Container.Resolve<WarningDialog>();
        window.Title = title;
        window.WarningMessage = message;
        await window.ShowDialog(this.MainWindow);
    }

    /// <inheritdoc />
    public async Task<YesNoCancelResult> ShowYesNoDialog(string title, string message, bool allowCancel) {
        var window = this.Container.Resolve<YesNoCancelDialog>();
        window.Title = title;
        window.Question = message;

        var result = await window.ShowDialog<YesNoCancelResult>(this.MainWindow);
        return result;
    }
}
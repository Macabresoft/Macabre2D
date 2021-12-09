namespace Macabresoft.AvaloniaEx;

using Avalonia.Controls;

/// <summary>
/// Helpers for controls..
/// </summary>
public static class ControlHelpers {
    /// <summary>
    /// Finds the first ancestor of the specified type.
    /// </summary>
    /// <param name="control">The control for which to find an ancestor.</param>
    /// <typeparam name="T">The type of control.</typeparam>
    /// <returns>The found control or null.</returns>
    public static T FindAncestor<T>(this IControl control) where T : class {
        T result = null;
        control = control?.Parent;
        while (control != null) {
            if (control is T foundControl) {
                result = foundControl;
                break;
            }

            control = control.Parent;
        }

        return result;
    }
}
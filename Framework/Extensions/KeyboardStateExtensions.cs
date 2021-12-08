namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Extension methods for <see cref="KeyboardState" />.
/// </summary>
public static class KeyboardStateExtensions {
    private static readonly Keys[] _modifierKeys = {
        Keys.LeftControl,
        Keys.RightControl,
        Keys.LeftAlt,
        Keys.RightAlt,
        Keys.LeftShift,
        Keys.RightShift
    };

    /// <summary>
    /// Determines whether all of the specified keys are currently down.
    /// </summary>
    /// <param name="keyboardState">State of the keyboard.</param>
    /// <param name="keys">The keys.</param>
    /// <returns><c>true</c> if all of the keys are down; otherwise, <c>false</c>.</returns>
    public static bool AreAllKeysDown(this KeyboardState keyboardState, params Keys[] keys) {
        return keys.All(x => keyboardState.IsKeyDown(x));
    }

    /// <summary>
    /// Determines whether any of the specified keys are currently down.
    /// </summary>
    /// <param name="keyboardState">State of the keyboard.</param>
    /// <param name="keys">The keys.</param>
    /// <returns><c>true</c> if any of the keys are down; otherwise, <c>false</c>.</returns>
    public static bool IsAnyKeyDown(this KeyboardState keyboardState, params Keys[] keys) {
        return keys.Any(x => keyboardState.IsKeyDown(x));
    }

    /// <summary>
    /// Determines whether or not a modifier key is currently down.
    /// </summary>
    /// <param name="keyboardState">State of the keyboard.</param>
    /// <returns><c>true</c> if a modifier key is down; otherwise, <c>false</c>.</returns>
    public static bool IsModifierKeyDown(this KeyboardState keyboardState) {
        return keyboardState.IsAnyKeyDown(_modifierKeys);
    }
}
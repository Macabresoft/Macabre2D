namespace Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Extension methods for <see cref="KeyboardState" />.
/// </summary>
public static class KeyboardStateExtensions {
    private static readonly Keys[] ModifierKeys = {
        Keys.LeftControl,
        Keys.RightControl,
        Keys.LeftAlt,
        Keys.RightAlt,
        Keys.LeftShift,
        Keys.RightShift
    };

    /// <param name="keyboardState">State of the keyboard.</param>
    extension(KeyboardState keyboardState) {
        /// <summary>
        /// Determines whether all the specified keys are currently down.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if all the keys are down; otherwise, <c>false</c>.</returns>
        public bool AreAllKeysDown(params Keys[] keys) {
            return keys.All(x => keyboardState.IsKeyDown(x));
        }

        /// <summary>
        /// Determines whether any of the specified keys are currently down.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if any of the keys are down; otherwise, <c>false</c>.</returns>
        public bool IsAnyKeyDown(params Keys[] keys) {
            return keys.Any(x => keyboardState.IsKeyDown(x));
        }

        /// <summary>
        /// Determines whether a modifier key is currently down.
        /// </summary>
        /// <returns><c>true</c> if a modifier key is down; otherwise, <c>false</c>.</returns>
        public bool IsModifierKeyDown() => keyboardState.IsAnyKeyDown(ModifierKeys);
    }
}
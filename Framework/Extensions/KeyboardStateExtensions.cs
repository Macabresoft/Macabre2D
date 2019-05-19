namespace Macabre2D.Framework.Extensions {

    using Microsoft.Xna.Framework.Input;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="KeyboardState"/>.
    /// </summary>
    public static class KeyboardStateExtensions {

        private static Keys[] _modifierKeys = new Keys[] {
            Keys.LeftControl,
            Keys.RightControl,
            Keys.LeftAlt,
            Keys.RightAlt,
            Keys.LeftShift,
            Keys.RightShift
        };

        /// <summary>
        /// Determines whether or not a modifier key is currently down.
        /// </summary>
        /// <param name="keyboardState">State of the keyboard.</param>
        /// <returns><c>true</c> if a modifier key is down; otherwise, <c>false</c>.</returns>
        public static bool IsModifierKeyDown(this KeyboardState keyboardState) {
            return KeyboardStateExtensions._modifierKeys.Any(x => keyboardState.IsKeyDown(x));
        }
    }
}
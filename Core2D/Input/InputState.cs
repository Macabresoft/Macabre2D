namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The input state for this frame.
    /// </summary>
    public struct InputState {

        /// <summary>
        /// The current keyboard state.
        /// </summary>
        public KeyboardState CurrentKeyboardState;

        /// <summary>
        /// The current mouse state.
        /// </summary>
        public MouseState CurrentMouseState;

        /// <summary>
        /// The previous keyboard state.
        /// </summary>
        public KeyboardState PreviousKeyboardState;

        /// <summary>
        /// The previous mouse state.
        /// </summary>
        public MouseState PreviousMouseState;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputState" /> struct.
        /// </summary>
        /// <param name="mouseState">State of the mouse.</param>
        /// <param name="keyboardState">State of the keyboard.</param>
        /// <param name="inputState">State of the input.</param>
        public InputState(MouseState mouseState, KeyboardState keyboardState, InputState inputState) {
            this.CurrentMouseState = mouseState;
            this.CurrentKeyboardState = keyboardState;
            this.PreviousMouseState = inputState.CurrentMouseState;
            this.PreviousKeyboardState = inputState.CurrentKeyboardState;
        }

        /// <inheritdoc />
        public static bool operator !=(InputState left, InputState right) {
            return !(left == right);
        }

        /// <inheritdoc />
        public static bool operator ==(InputState left, InputState right) {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) {
            var result = false;
            if (obj is InputState other) {
                result = other.CurrentKeyboardState == this.CurrentKeyboardState && other.CurrentMouseState == this.CurrentMouseState &&
                    other.PreviousKeyboardState == this.PreviousKeyboardState && other.PreviousMouseState == this.PreviousMouseState;
            }

            return result;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return System.HashCode.Combine(this.CurrentKeyboardState, this.CurrentMouseState, this.PreviousKeyboardState, this.PreviousMouseState);
        }
    }
}
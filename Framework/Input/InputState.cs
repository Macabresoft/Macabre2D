namespace Macabresoft.Macabre2D.Framework;

using System;
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

    /// <summary>
    /// Gets a value indicating whether not the mouse button is newly pressed as of the current frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is newly pressed as of the current frame.</returns>
    public bool IsButtonNewlyPressed(MouseButton button) {
        return this.GetPreviousButtonState(button) == ButtonState.Released && this.GetCurrentButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the button is released.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is being held.</returns>
    public bool IsButtonHeld(MouseButton button) {
        return this.GetCurrentButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is newly released as of the current frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is newly released as of the current frame.</returns>
    public bool IsButtonNewlyReleased(MouseButton button) {
        return this.GetPreviousButtonState(button) == ButtonState.Pressed && this.GetCurrentButtonState(button) == ButtonState.Released;
    }

    /// <summary>
    /// Gets the mouse button's current state.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>The mouse button's current state.</returns>
    public ButtonState GetCurrentButtonState(MouseButton button) {
        return this.GetButtonState(button, this.CurrentMouseState);
    }

    /// <summary>
    /// Gets the mouse button's state in the previous frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>The mouse button's state in the previous frame.</returns>
    public ButtonState GetPreviousButtonState(MouseButton button) {
        return this.GetButtonState(button, this.PreviousMouseState);
    }

    private ButtonState GetButtonState(MouseButton button, MouseState state) {
        switch (button) {
            case MouseButton.Left:
                return state.LeftButton;
            case MouseButton.Middle:
                return state.MiddleButton;
            case MouseButton.Right:
                return state.RightButton;
            case MouseButton.XButton1:
                return state.XButton1;
            case MouseButton.XButton2:
                return state.XButton2;
            default:
                return ButtonState.Released;
        }
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
        return HashCode.Combine(this.CurrentKeyboardState, this.CurrentMouseState, this.PreviousKeyboardState, this.PreviousMouseState);
    }
}
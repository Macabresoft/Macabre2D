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
    public bool IsMouseButtonNewlyPressed(MouseButton button) {
        return this.GetPreviousMouseButtonState(button) == ButtonState.Released && this.GetCurrentMouseButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is newly pressed as of the current frame.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is newly pressed as of the current frame.</returns>
    public bool IsKeyNewlyPressed(Keys key) {
        return this.GetPreviousKeyState(key) == KeyState.Up && this.GetCurrentKeyState(key) == KeyState.Down;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the button is released.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is being held.</returns>
    public bool IsMouseButtonHeld(MouseButton button) {
        return this.GetCurrentMouseButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the key is released.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is being held.</returns>
    public bool IsKeyHeld(Keys key) {
        return this.GetCurrentKeyState(key) == KeyState.Down;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is newly released as of the current frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is newly released as of the current frame.</returns>
    public bool IsMouseButtonNewlyReleased(MouseButton button) {
        return this.GetPreviousMouseButtonState(button) == ButtonState.Pressed && this.GetCurrentMouseButtonState(button) == ButtonState.Released;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is newly released as of the current frame.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is newly released as of the current frame.</returns>
    public bool IsKeyNewlyReleased(Keys key) {
        return this.GetPreviousKeyState(key) == KeyState.Down && this.GetCurrentKeyState(key) == KeyState.Up;
    }

    /// <summary>
    /// Gets the mouse button's current state.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>The mouse button's current state.</returns>
    public ButtonState GetCurrentMouseButtonState(MouseButton button) {
        return GetMouseButtonState(button, this.CurrentMouseState);
    }

    /// <summary>
    /// Gets the key's current state.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>The key's current state.</returns>
    private KeyState GetCurrentKeyState(Keys key) {
        return GetKeyState(key, this.CurrentKeyboardState);
    }

    /// <summary>
    /// Gets the mouse button's state in the previous frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>The mouse button's state in the previous frame.</returns>
    public ButtonState GetPreviousMouseButtonState(MouseButton button) {
        return GetMouseButtonState(button, this.PreviousMouseState);
    }

    /// <summary>
    /// Gets the key's state in the previous frame.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>The key's state in the previous frame.</returns>
    public KeyState GetPreviousKeyState(Keys key) {
        return GetKeyState(key, this.PreviousKeyboardState);
    }

    private static ButtonState GetMouseButtonState(MouseButton button, MouseState state) {
        return button switch {
            MouseButton.Left => state.LeftButton,
            MouseButton.Middle => state.MiddleButton,
            MouseButton.Right => state.RightButton,
            MouseButton.XButton1 => state.XButton1,
            MouseButton.XButton2 => state.XButton2,
            _ => ButtonState.Released
        };
    }

    private static KeyState GetKeyState(Keys key, KeyboardState state) {
        return state.IsKeyDown(key) ? KeyState.Down : KeyState.Up;
    }

    /// <inheritdoc cref="object" />
    public static bool operator !=(InputState left, InputState right) {
        return !(left == right);
    }

    /// <inheritdoc cref="object" />
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
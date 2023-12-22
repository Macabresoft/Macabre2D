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
    /// The previous game pad state.
    /// </summary>
    public GamePadState PreviousGamePadState;

    /// <summary>
    /// The current game pad state.
    /// </summary>
    public GamePadState CurrentGamePadState;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputState" /> struct.
    /// </summary>
    /// <param name="mouseState">State of the mouse.</param>
    /// <param name="keyboardState">State of the keyboard.</param>
    /// <param name="gamePadState">State of the game pad.</param>
    /// <param name="previousInputState">Previous state of the input.</param>
    public InputState(MouseState mouseState, KeyboardState keyboardState, GamePadState gamePadState, InputState previousInputState) {
        this.CurrentMouseState = mouseState;
        this.CurrentKeyboardState = keyboardState;
        this.CurrentGamePadState = gamePadState;
        this.PreviousMouseState = previousInputState.CurrentMouseState;
        this.PreviousKeyboardState = previousInputState.CurrentKeyboardState;
        this.PreviousGamePadState = previousInputState.CurrentGamePadState;
    }

    /// <summary>
    /// Checks whether any game pad buttons are being pressed.
    /// </summary>
    /// <returns>A value indicating whether or not any game pad buttons are being pressed.</returns>
    public bool IsGamePadActive() {
        // The D-Pad and Thumbsticks aren't a part of Buttons.GetHashCode() so they need a special check.
        return this.CurrentGamePadState.IsConnected &&
               (this.CurrentGamePadState.Buttons.GetHashCode() != 0 ||
                this.IsGamePadButtonHeld(Buttons.DPadLeft) ||
                this.IsGamePadButtonHeld(Buttons.DPadUp) ||
                this.IsGamePadButtonHeld(Buttons.DPadRight) ||
                this.IsGamePadButtonHeld(Buttons.DPadDown) ||
                this.IsGamePadButtonHeld(Buttons.LeftThumbstickLeft) ||
                this.IsGamePadButtonHeld(Buttons.LeftThumbstickUp) ||
                this.IsGamePadButtonHeld(Buttons.LeftThumbstickRight) ||
                this.IsGamePadButtonHeld(Buttons.LeftThumbstickDown) ||
                this.IsGamePadButtonHeld(Buttons.RightThumbstickLeft) ||
                this.IsGamePadButtonHeld(Buttons.RightThumbstickUp) ||
                this.IsGamePadButtonHeld(Buttons.RightThumbstickRight) ||
                this.IsGamePadButtonHeld(Buttons.RightThumbstickDown));
    }

    /// <summary>
    /// Checks whether any keyboard keys are being pressed.
    /// </summary>
    /// <returns>A value indicating whether or not any keyboard keys are being pressed.</returns>
    public bool IsKeyboardActive() {
        return this.CurrentKeyboardState.GetPressedKeyCount() > 0;
    }

    /// <summary>
    /// Gets the button's current state.
    /// </summary>
    /// <param name="button">The game pad button.</param>
    /// <returns>The key's current state.</returns>
    private ButtonState GetCurrentGamePadButtonState(Buttons button) {
        return GetGamePadButtonState(button, this.CurrentGamePadState);
    }

    /// <summary>
    /// Gets the button's state in the previous frame.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>The button's state in the previous frame.</returns>
    public ButtonState GetPreviousGamePadButtonState(Buttons button) {
        return GetGamePadButtonState(button, this.PreviousGamePadState);
    }

    /// <summary>
    /// Gets a value indicating whether not the button is newly pressed as of the current frame.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A value indicating whether not the button is newly pressed as of the current frame.</returns>
    public bool IsGamePadButtonNewlyPressed(Buttons button) {
        return button != Buttons.None && this.GetPreviousGamePadButtonState(button) == ButtonState.Released && this.GetCurrentGamePadButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the button is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the button is released.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A value indicating whether not the button is being held.</returns>
    public bool IsGamePadButtonHeld(Buttons button) {
        return button != Buttons.None && this.GetCurrentGamePadButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the button is newly released as of the current frame.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <returns>A value indicating whether not the button is newly released as of the current frame.</returns>
    public bool IsGamePadButtonNewlyReleased(Buttons button) {
        return button != Buttons.None && this.GetPreviousGamePadButtonState(button) == ButtonState.Pressed && this.GetCurrentGamePadButtonState(button) == ButtonState.Released;
    }

    private static ButtonState GetGamePadButtonState(Buttons button, GamePadState state) {
        return button != Buttons.None && state.IsButtonDown(button) ? ButtonState.Pressed : ButtonState.Released;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is newly pressed as of the current frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is newly pressed as of the current frame.</returns>
    public bool IsMouseButtonNewlyPressed(MouseButton button) {
        return button != MouseButton.None && this.GetPreviousMouseButtonState(button) == ButtonState.Released && this.GetCurrentMouseButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is newly pressed as of the current frame.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is newly pressed as of the current frame.</returns>
    public bool IsKeyNewlyPressed(Keys key) {
        return key != Keys.None && this.GetPreviousKeyState(key) == KeyState.Up && this.GetCurrentKeyState(key) == KeyState.Down;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the button is released.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is being held.</returns>
    public bool IsMouseButtonHeld(MouseButton button) {
        return button != MouseButton.None && this.GetCurrentMouseButtonState(button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is being held. This includes the first frame of being pressed and
    /// all subsequent frames until the key is released.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is being held.</returns>
    public bool IsKeyHeld(Keys key) {
        return key != Keys.None && this.GetCurrentKeyState(key) == KeyState.Down;
    }

    /// <summary>
    /// Gets a value indicating whether not the mouse button is newly released as of the current frame.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <returns>A value indicating whether not the mouse button is newly released as of the current frame.</returns>
    public bool IsMouseButtonNewlyReleased(MouseButton button) {
        return button != MouseButton.None && this.GetPreviousMouseButtonState(button) == ButtonState.Pressed && this.GetCurrentMouseButtonState(button) == ButtonState.Released;
    }

    /// <summary>
    /// Gets a value indicating whether not the key is newly released as of the current frame.
    /// </summary>
    /// <param name="key">The keyboard key.</param>
    /// <returns>A value indicating whether not the key is newly released as of the current frame.</returns>
    public bool IsKeyNewlyReleased(Keys key) {
        return key != Keys.None && this.GetPreviousKeyState(key) == KeyState.Down && this.GetCurrentKeyState(key) == KeyState.Up;
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
        return key != Keys.None && state.IsKeyDown(key) ? KeyState.Down : KeyState.Up;
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
namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// The state of an <see cref="InputAction" /> during a frame.
/// </summary>
public enum InputActionState {
    Pressed,
    Held,
    Released,
    None
}

/// <summary>
/// The kind of input.
/// </summary>
[Flags]
public enum InputKind : byte {
    None,
    GamePad = 1 << 0,
    Keyboard = 1 << 1,
    Mouse = 1 << 2
}

/// <summary>
/// Interface for a <see cref="GameSystem" /> that handles input.
/// </summary>
public interface IInputSystem {

    /// <summary>
    /// Gets the horizontal axis value.
    /// </summary>
    float HorizontalAxis { get; }

    /// <summary>
    /// Gets the vertical axis value.
    /// </summary>
    float VerticalAxis { get; }

    /// <summary>
    /// Gets the input action's state for the current frame.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The input action's state.</returns>
    InputActionState GetInputActionState(InputAction action);

    /// <summary>
    /// Gets the input action's state for the current frame.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="inputKind">A value indicating which inputs to check.</param>
    /// <returns>The input action's state.</returns>
    InputActionState GetInputActionState(InputAction action, InputKind inputKind);

    /// <summary>
    /// Gets a value indicating whether a given action is being held.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A value indicating whether the action is being held.</returns>
    bool IsHeld(InputAction action);

    /// <summary>
    /// Gets a value indicating whether a given action is being newly pressed.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A value indicating whether the action is being newly pressed.</returns>
    bool IsPressed(InputAction action);

    /// <summary>
    /// Gets a value indicating whether a given action is being newly pressed.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="inputKind">A value indicating which inputs to check.</param>
    /// <returns>A value indicating whether the action is being newly pressed.</returns>
    bool IsPressed(InputAction action, InputKind inputKind);

    /// <summary>
    /// Gets a value indicating whether a given action is being newly released.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A value indicating whether the action is being newly released.</returns>
    bool IsReleased(InputAction action);
}

/// <summary>
/// A system which handles basic input operations on a per-frame basis.
/// </summary>
public class InputSystem : UpdateSystem, IInputSystem {
    private readonly IDictionary<InputAction, InputActionState> _actionToButtonState = new Dictionary<InputAction, InputActionState>();
    private InputState _inputState;

    /// <inheritdoc />
    public float HorizontalAxis { get; private set; }

    /// <inheritdoc />
    public override UpdateSystemKind Kind => UpdateSystemKind.PreUpdate;

    /// <inheritdoc />
    public float VerticalAxis { get; private set; }

    /// <inheritdoc />
    public InputActionState GetInputActionState(InputAction action) {
        if (!this._actionToButtonState.TryGetValue(action, out var result)) {
            this.Game.InputSettings.TryGetBindings(action, out var primaryButton, out var secondaryButton, out var key, out var mouseButton);
            result = this.ResolveState(action, primaryButton, secondaryButton, key, mouseButton, true);
        }

        return result;
    }

    /// <inheritdoc />
    public InputActionState GetInputActionState(InputAction action, InputKind inputKind) {
        var result = InputActionState.None;
        if (inputKind.HasFlag(InputKind.GamePad)) {
            this.Game.InputSettings.TryGetBindings(action, out var primaryButton, out var secondaryButton);
            result = this.ResolveState(action, primaryButton, secondaryButton, Keys.None, MouseButton.None, false);
        }

        if (result == InputActionState.None && inputKind.HasFlag(InputKind.Keyboard)) {
            this.Game.InputSettings.TryGetBindings(action, out Keys key);
            result = this.ResolveState(action, Buttons.None, Buttons.None, key, MouseButton.None, false);
        }

        if (result == InputActionState.None && inputKind.HasFlag(InputKind.Mouse)) {
            this.Game.InputSettings.TryGetBindings(action, out MouseButton button);
            result = this.ResolveState(action, Buttons.None, Buttons.None, Keys.None, button, false);
        }

        return result;
    }

    /// <inheritdoc />
    public bool IsHeld(InputAction action) => this.GetInputActionState(action) is InputActionState.Held or InputActionState.Pressed;

    /// <inheritdoc />
    public bool IsPressed(InputAction action) => this.GetInputActionState(action) is InputActionState.Pressed;

    /// <inheritdoc />
    public bool IsPressed(InputAction action, InputKind inputKind) => this.GetInputActionState(action, inputKind) is InputActionState.Pressed;

    /// <inheritdoc />
    public bool IsReleased(InputAction action) => this.GetInputActionState(action) is InputActionState.Released;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this._actionToButtonState.Clear();
        this._inputState = inputState;
        this.HorizontalAxis = 0f;
        this.VerticalAxis = 0f;

        var downState = this.GetInputActionState(InputAction.Down);
        if (downState == InputActionState.Pressed) {
            this._actionToButtonState[InputAction.Down] = InputActionState.Pressed;
            this.VerticalAxis -= 1f;
        }
        else if (downState == InputActionState.Held) {
            this._actionToButtonState[InputAction.Down] = InputActionState.Held;
            this.VerticalAxis -= 1f;
        }

        var upState = this.GetInputActionState(InputAction.Up);
        if (upState == InputActionState.Pressed) {
            this._actionToButtonState[InputAction.Up] = InputActionState.Pressed;
            this.VerticalAxis += 1f;
        }
        else if (upState == InputActionState.Held) {
            this._actionToButtonState[InputAction.Up] = InputActionState.Held;
            this.VerticalAxis += 1f;
        }

        var leftState = this.GetInputActionState(InputAction.Left);
        if (leftState == InputActionState.Pressed) {
            this._actionToButtonState[InputAction.Left] = InputActionState.Pressed;
            this.HorizontalAxis -= 1f;
        }
        else if (leftState == InputActionState.Held) {
            this.HorizontalAxis -= 1f;
            this._actionToButtonState[InputAction.Left] = InputActionState.Held;
        }

        var rightState = this.GetInputActionState(InputAction.Right);
        if (rightState == InputActionState.Pressed) {
            this._actionToButtonState[InputAction.Right] = InputActionState.Pressed;
            this.HorizontalAxis += 1f;
        }
        else if (rightState == InputActionState.Held) {
            this.HorizontalAxis += 1f;
            this._actionToButtonState[InputAction.Right] = InputActionState.Held;
        }
    }

    private bool IsHeld(Buttons primaryButton, Buttons secondaryButton, Keys key, MouseButton mouseButton) =>
        this.IsHeld(primaryButton, secondaryButton) ||
        this.IsHeld(key, mouseButton);

    private bool IsHeld(Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyHeld(key) ||
        this._inputState.IsMouseButtonHeld(mouseButton);

    private bool IsHeld(Buttons primaryButton, Buttons secondaryButton) =>
        this._inputState.IsGamePadButtonHeld(primaryButton) ||
        this._inputState.IsGamePadButtonHeld(secondaryButton);

    private bool IsNewlyPressed(Buttons primaryButton, Buttons secondaryButton, Keys key, MouseButton mouseButton) =>
        this.IsNewlyPressed(primaryButton, secondaryButton) ||
        this.IsNewlyPressed(key, mouseButton);

    private bool IsNewlyPressed(Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyNewlyPressed(key) ||
        this._inputState.IsMouseButtonNewlyPressed(mouseButton);

    private bool IsNewlyPressed(Buttons primaryButton, Buttons secondaryButton) =>
        this._inputState.IsGamePadButtonNewlyPressed(primaryButton) ||
        this._inputState.IsGamePadButtonNewlyPressed(secondaryButton);

    private bool IsNewlyReleased(Buttons primaryButton, Buttons secondaryButton, Keys key, MouseButton mouseButton) =>
        this.IsNewlyReleased(primaryButton, secondaryButton) ||
        this.IsNewlyReleased(key, mouseButton);

    private bool IsNewlyReleased(Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyNewlyReleased(key) ||
        this._inputState.IsMouseButtonNewlyReleased(mouseButton);

    private bool IsNewlyReleased(Buttons primaryButton, Buttons secondaryButton) =>
        this._inputState.IsGamePadButtonNewlyReleased(primaryButton) ||
        this._inputState.IsGamePadButtonNewlyReleased(secondaryButton);

    private InputActionState ResolveState(
        InputAction action,
        Buttons primaryButton,
        Buttons secondaryButton,
        Keys key,
        MouseButton mouseButton,
        bool cacheResult) {
        var result = InputActionState.None;
        if (this.Game.InputSettings.DesiredInputDevice == InputDevice.Auto || this.Game.Project.AllowInputRegardlessOfDevice) {
            if (this.IsNewlyPressed(primaryButton, secondaryButton, key, mouseButton)) {
                result = InputActionState.Pressed;
            }
            else if (this.IsHeld(primaryButton, secondaryButton, key, mouseButton)) {
                result = InputActionState.Held;
            }
            else if (this.IsNewlyReleased(primaryButton, secondaryButton, key, mouseButton)) {
                result = InputActionState.Released;
            }
        }
        else if (this.Game.InputSettings.DesiredInputDevice == InputDevice.GamePad) {
            if (this.IsNewlyPressed(primaryButton, secondaryButton)) {
                result = InputActionState.Pressed;
            }
            else if (this.IsHeld(primaryButton, secondaryButton)) {
                result = InputActionState.Held;
            }
            else if (this.IsNewlyReleased(primaryButton, secondaryButton)) {
                result = InputActionState.Released;
            }
        }
        else if (this.Game.InputSettings.DesiredInputDevice == InputDevice.KeyboardMouse) {
            if (this.IsNewlyPressed(key, mouseButton)) {
                result = InputActionState.Pressed;
            }
            else if (this.IsHeld(key, mouseButton)) {
                result = InputActionState.Held;
            }
            else if (this.IsNewlyReleased(key, mouseButton)) {
                result = InputActionState.Released;
            }
        }

        if (cacheResult) {
            this._actionToButtonState[action] = result;
        }

        return result;
    }
}
namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Gets a value indicating whether a given action is being newly released.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A value indicating whether the action is being newly released.</returns>
    bool IsReleased(InputAction action);
}

/// <summary>
/// A system which handles basic input operations on a per-frame basis.
/// </summary>
public class InputSystem : GameSystem, IInputSystem {
    private readonly IDictionary<InputAction, InputActionState> _actionToButtonState = new Dictionary<InputAction, InputActionState>();
    private InputState _inputState;

    public static IInputSystem Empty { get; } = new EmptyInputSystem();

    /// <inheritdoc />
    public override GameSystemKind Kind => GameSystemKind.PreUpdate;

    /// <inheritdoc />
    public float HorizontalAxis { get; private set; }

    /// <inheritdoc />
    public float VerticalAxis { get; private set; }

    /// <inheritdoc />
    public InputActionState GetInputActionState(InputAction action) {
        if (!this._actionToButtonState.TryGetValue(action, out var result)) {
            this.Game.InputBindings.TryGetBindings(action, out var buttons, out var key, out var mouseButton);
            result = this.ResolveState(action, buttons, key, mouseButton);
        }

        return result;
    }

    public override void Initialize(IScene scene) {
        base.Initialize(scene);
    }

    /// <inheritdoc />
    public bool IsHeld(InputAction action) => this.GetInputActionState(action) is InputActionState.Held or InputActionState.Pressed;

    /// <inheritdoc />
    public bool IsPressed(InputAction action) => this.GetInputActionState(action) is InputActionState.Pressed;

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

    private bool IsAnyGamePadButtonHeld(Buttons gamePadButtons) {
        if (gamePadButtons == Buttons.None) {
            return false;
        }

        var values = Enum.GetValues<Buttons>().Where(x => gamePadButtons.HasFlag(x)).ToList();
        return values.Any(this._inputState.IsGamePadButtonHeld);
    }

    private bool IsAnyGamePadButtonNewlyPressed(Buttons gamePadButtons) {
        if (gamePadButtons == Buttons.None) {
            return false;
        }

        var values = Enum.GetValues<Buttons>().Where(x => gamePadButtons.HasFlag(x)).ToList();
        return values.Any(this._inputState.IsGamePadButtonNewlyPressed);
    }

    private bool IsAnyGamePadButtonNewlyReleased(Buttons gamePadButtons) {
        if (gamePadButtons == Buttons.None) {
            return false;
        }

        var values = Enum.GetValues<Buttons>().Where(x => gamePadButtons.HasFlag(x)).ToList();
        return values.Any(this._inputState.IsGamePadButtonNewlyReleased);
    }

    private bool IsHeld(Buttons gamePadButtons, Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyHeld(key) || this.IsAnyGamePadButtonHeld(gamePadButtons) ||
        this._inputState.IsMouseButtonHeld(mouseButton);

    private bool IsNewlyPressed(Buttons gamePadButtons, Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyNewlyPressed(key) || this.IsAnyGamePadButtonNewlyPressed(gamePadButtons) ||
        this._inputState.IsMouseButtonNewlyPressed(mouseButton);

    private bool IsNewlyReleased(Buttons gamePadButtons, Keys key, MouseButton mouseButton) =>
        this._inputState.IsKeyNewlyReleased(key) || this.IsAnyGamePadButtonNewlyReleased(gamePadButtons) ||
        this._inputState.IsMouseButtonNewlyReleased(mouseButton);

    private InputActionState ResolveState(InputAction action, Buttons buttons, Keys key, MouseButton mouseButton) {
        var result = InputActionState.None;
        if (this.IsNewlyPressed(buttons, key, mouseButton)) {
            result = InputActionState.Pressed;
        }
        else if (this.IsHeld(buttons, key, mouseButton)) {
            result = InputActionState.Held;
        }
        else if (this.IsNewlyReleased(buttons, key, mouseButton)) {
            result = InputActionState.Released;
        }

        this._actionToButtonState[action] = result;
        return result;
    }

    private sealed class EmptyInputSystem : IInputSystem {
        public float HorizontalAxis => 0f;
        public float VerticalAxis => 0f;
        public InputActionState GetInputActionState(InputAction action) => InputActionState.None;
        public bool IsHeld(InputAction action) => false;
        public bool IsPressed(InputAction action) => false;
        public bool IsReleased(InputAction action) => false;
    }
}
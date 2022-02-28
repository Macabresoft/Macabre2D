namespace Macabresoft.Macabre2D.Libraries.Platformer;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// The state of a button.
/// </summary>
public enum ButtonState {
    Pressed,
    Held,
    None
}

/// <summary>
/// A manager to simplify input related actions.
/// </summary>
public sealed class InputManager {

    /// <summary>
    /// Gets the jump state.
    /// </summary>
    public ButtonState JumpState { get; private set; } = ButtonState.None;

    /// <summary>
    /// Gets the horizontal axis value between -1 and 1.
    /// </summary>
    public float HorizontalAxis { get; private set; } = 0f;

    /// <summary>
    /// Updates values for the current frame.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    public void Update(InputState inputState) {
        if (inputState.IsKeyNewlyPressed(Keys.Space)) {
            this.JumpState = ButtonState.Pressed;
        }
        else if (inputState.IsKeyHeld(Keys.Space)) {
            this.JumpState = ButtonState.Held;
        }
        else {
            this.JumpState = ButtonState.None;
        }

        this.HorizontalAxis = 0f;

        if (inputState.IsKeyHeld(Keys.A)) {
            this.HorizontalAxis -= 1f;
        }

        if (inputState.IsKeyHeld(Keys.D)) {
            this.HorizontalAxis += 1f;
        }
    }
}
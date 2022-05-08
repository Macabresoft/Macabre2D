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
public class InputManager {
    /// <summary>
    /// Gets or sets the dead zone for analog input.
    /// </summary>
    /// <remarks>
    /// TODO: make this customizable.
    /// </remarks>
    public float DeadZone { get; set; } = 0.1f;

    /// <summary>
    /// Gets the horizontal axis value between -1 and 1.
    /// </summary>
    public float HorizontalAxis { get; private set; }

    /// <summary>
    /// Gets the jump state.
    /// </summary>
    public ButtonState JumpState { get; private set; } = ButtonState.None;

    /// <summary>
    /// Updates values for the current frame.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    public virtual void Update(InputState inputState) {
        if (inputState.IsKeyNewlyPressed(Keys.Space) || inputState.IsGamePadButtonNewlyPressed(Buttons.A)) {
            this.JumpState = ButtonState.Pressed;
        }
        else if (inputState.IsKeyHeld(Keys.Space) || inputState.IsGamePadButtonHeld(Buttons.A)) {
            this.JumpState = ButtonState.Held;
        }
        else {
            this.JumpState = ButtonState.None;
        }

        this.HorizontalAxis = 0f;

        if (inputState.IsKeyHeld(Keys.A) || inputState.IsGamePadButtonHeld(Buttons.DPadLeft) || inputState.CurrentGamePadState.ThumbSticks.Left.X < -this.DeadZone) {
            this.HorizontalAxis -= 1f;
        }

        if (inputState.IsKeyHeld(Keys.D) || inputState.IsGamePadButtonHeld(Buttons.DPadRight) || inputState.CurrentGamePadState.ThumbSticks.Left.X > this.DeadZone) {
            this.HorizontalAxis += 1f;
        }
    }
}
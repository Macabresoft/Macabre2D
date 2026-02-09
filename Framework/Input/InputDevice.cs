namespace Macabre2D.Framework;

/// <summary>
/// The input device used when interacting with the game.
/// </summary>
public enum InputDevice {
    /// <summary>
    /// Will automatically detect whether or not a game pad or keyboard is active and use that.
    /// </summary>
    Auto,
    
    /// <summary>
    /// Will only accept inputs from a game pad.
    /// </summary>
    GamePad,
    
    /// <summary>
    /// Will only accept inputs from a keyboard or mouse.
    /// </summary>
    KeyboardMouse
}
namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

/// <summary>
/// A class which defines input bindings between <see cref="InputAction" /> and <see cref="Keys" />, <see cref="Buttons" />, and <see cref="MouseButton" />.
/// </summary>
[DataContract]
public class InputBindings : VersionedData {
    /// <summary>
    /// The settings file name.
    /// </summary>
    public const string SettingsFileName = "InputBindings.m2d";

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Buttons> _gamePadBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Keys> _keyBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, MouseButton> _mouseBindings = new();

    /// <summary>
    /// Initializes a new instance of <see cref="InputBindings" />.
    /// </summary>
    public InputBindings() {
    }

    private InputBindings(Dictionary<InputAction, Buttons> gamePadBindings, Dictionary<InputAction, Keys> keyBindings, Dictionary<InputAction, MouseButton> mouseBindings) {
        this._gamePadBindings = new Dictionary<InputAction, Buttons>(gamePadBindings);
        this._keyBindings = new Dictionary<InputAction, Keys>(keyBindings);
        this._mouseBindings = new Dictionary<InputAction, MouseButton>(mouseBindings);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the left analog stick is enabled.
    /// </summary>
    [DataMember]
    public bool IsLeftAnalogEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the mouse is enabled.
    /// </summary>
    [DataMember]
    public bool IsMouseEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the right analog stick is enabled.
    /// </summary>
    [DataMember]
    public bool IsRightAnalogEnabled { get; set; } = true;

    /// <summary>
    /// Clears all bindings for the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ClearBindings(InputAction action) {
        this.RemoveGamePadBinding(action);
        this.RemoveKeyBinding(action);
        this.RemoveMouseBinding(action);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public InputBindings Clone() {
        return new InputBindings(this._gamePadBindings, this._keyBindings, this._mouseBindings) {
            IsMouseEnabled = this.IsMouseEnabled,
            IsLeftAnalogEnabled = this.IsLeftAnalogEnabled,
            IsRightAnalogEnabled = this.IsRightAnalogEnabled
        };
    }

    /// <summary>
    /// Removes a game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveGamePadBinding(InputAction action) {
        this._gamePadBindings.Remove(action);
    }

    /// <summary>
    /// Removes a key binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveKeyBinding(InputAction action) {
        this._keyBindings.Remove(action);
    }

    /// <summary>
    /// Removes a mouse binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveMouseBinding(InputAction action) {
        this._mouseBindings.Remove(action);
    }

    /// <summary>
    /// Sets a game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="buttons">The buttons.</param>
    public void SetGamePadBinding(InputAction action, Buttons buttons) {
        this._gamePadBindings[action] = buttons;
    }

    /// <summary>
    /// Sets a key binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="key">The key.</param>
    public void SetKeyBinding(InputAction action, Keys key) {
        this._keyBindings[action] = key;
    }

    /// <summary>
    /// Sets a mouse binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="button">The button.</param>
    public void SetMouseBinding(InputAction action, MouseButton button) {
        this._mouseBindings[action] = button;
    }

    /// <summary>
    /// Gets all possible bindings for an action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="gamePadButtons">The game pad buttons binding.</param>
    /// <param name="key">The key binding.</param>
    /// <param name="mouseButton">The mouse binding.</param>
    /// <returns>A value indicating whether or not any of the bindings exist.</returns>
    public bool TryGetBindings(InputAction action, out Buttons gamePadButtons, out Keys key, out MouseButton mouseButton) {
        var result = this._gamePadBindings.TryGetValue(action, out gamePadButtons);
        result = this._keyBindings.TryGetValue(action, out key) || result;
        result = this._mouseBindings.TryGetValue(action, out mouseButton) || result;
        return result;
    }
}
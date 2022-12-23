namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

/// <summary>
/// A class which defines input bindings between <see cref="InputAction" /> and <see cref="Keys" />, <see cref="Buttons" />, and <see cref="MouseButton" />.
/// </summary>
[DataContract]
public class InputBindings {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Buttons> _controllerBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Keys> _keyBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, MouseButton> _mouseBindings = new();

    /// <summary>
    /// Sets a controller binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="button">The button.</param>
    public void SetControllerBinding(InputAction action, Buttons button) {
        this._controllerBindings[action] = button;
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
    /// Clears all bindings for the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ClearBindings(InputAction action) {
        this.RemoveControllerBinding(action);
        this.RemoveKeyBinding(action);
        this.RemoveMouseBinding(action);
    }

    /// <summary>
    /// Removes a controller binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveControllerBinding(InputAction action) {
        this._controllerBindings.Remove(action);
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
    /// Gets all possible bindings for an action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="controllerButton">The controller button binding.</param>
    /// <param name="key">The key binding.</param>
    /// <param name="mouseButton">The mouse binding.</param>
    /// <returns>A value indicating whether or not any of the bindings exist.</returns>
    public bool TryGetBindings(InputAction action, out Buttons controllerButton, out Keys key, out MouseButton mouseButton) {
        var result = this._controllerBindings.TryGetValue(action, out controllerButton);
        result = this._keyBindings.TryGetValue(action, out key) || result;
        result = this._mouseBindings.TryGetValue(action, out mouseButton) || result;
        return result;
    }
}
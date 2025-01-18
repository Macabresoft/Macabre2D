namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

/// <summary>
/// A class which defines input bindings between <see cref="InputAction" /> and <see cref="Keys" />, <see cref="Buttons" />
/// , and <see cref="MouseButton" />.
/// </summary>
[DataContract]
public class InputBindings {

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Keys> _keyBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, MouseButton> _mouseBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Buttons> _primaryGamePadBindings = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, Buttons> _secondaryGamePadBindings = new();

    /// <summary>
    /// Called when a binding changes for a specific <see cref="InputAction" />.
    /// </summary>
    public event EventHandler<InputAction>? BindingChanged;

    /// <summary>
    /// Initializes a new instance of <see cref="InputBindings" />.
    /// </summary>
    public InputBindings() {
    }

    private InputBindings(
        Dictionary<InputAction, Buttons> primaryGamePadBindings,
        Dictionary<InputAction, Buttons> secondaryGamePadBindings,
        Dictionary<InputAction, Keys> keyBindings,
        Dictionary<InputAction, MouseButton> mouseBindings,
        bool isMouseEnabled) {
        this._primaryGamePadBindings = new Dictionary<InputAction, Buttons>(primaryGamePadBindings);
        this._secondaryGamePadBindings = new Dictionary<InputAction, Buttons>(secondaryGamePadBindings);
        this._keyBindings = new Dictionary<InputAction, Keys>(keyBindings);
        this._mouseBindings = new Dictionary<InputAction, MouseButton>(mouseBindings);
        this.IsMouseEnabled = isMouseEnabled;
    }

    /// <summary>
    /// Gets or sets the desired game pad.
    /// </summary>
    [DataMember]
    public GamePadDisplay DesiredGamePad { get; set; } = GamePadDisplay.X;

    /// <summary>
    /// Gets or sets the desired input device.
    /// </summary>
    [DataMember]
    public InputDevice DesiredInputDevice { get; set; } = InputDevice.Auto;

    /// <summary>
    /// Gets or sets a value indicating whether the mouse is enabled.
    /// </summary>
    [DataMember]
    public bool IsMouseEnabled { get; set; } = true;

    /// <summary>
    /// Clears all bindings for the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ClearBindings(InputAction action) {
        this.RemovePrimaryGamePadBinding(action);
        this.RemoveSecondaryGamePadBinding(action);
        this.RemoveKeyBinding(action);
        this.RemoveMouseBinding(action);
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public InputBindings Clone() => new(
        this._primaryGamePadBindings,
        this._secondaryGamePadBindings,
        this._keyBindings,
        this._mouseBindings,
        this.IsMouseEnabled);

    /// <summary>
    /// Copies settings to another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    public void CopyTo(InputBindings other) {
        other._primaryGamePadBindings.Clear();
        other._keyBindings.Clear();
        other._mouseBindings.Clear();

        foreach (var (gamePadAction, gamePadButton) in this._primaryGamePadBindings) {
            other._primaryGamePadBindings.Add(gamePadAction, gamePadButton);
        }

        foreach (var (gamePadAction, gamePadButton) in this._secondaryGamePadBindings) {
            other._secondaryGamePadBindings.Add(gamePadAction, gamePadButton);
        }

        foreach (var (keyAction, key) in this._keyBindings) {
            other._keyBindings.Add(keyAction, key);
        }

        foreach (var (mouseAction, mouseButton) in this._mouseBindings) {
            other._mouseBindings.Add(mouseAction, mouseButton);
        }

        other.IsMouseEnabled = this.IsMouseEnabled;
    }

    /// <summary>
    /// Removes a key binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveKeyBinding(InputAction action) {
        this._keyBindings.Remove(action);
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Removes a mouse binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveMouseBinding(InputAction action) {
        this._mouseBindings.Remove(action);
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Removes a primary game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemovePrimaryGamePadBinding(InputAction action) {
        this._primaryGamePadBindings.Remove(action);
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Removes a secpmdary game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    public void RemoveSecondaryGamePadBinding(InputAction action) {
        this._secondaryGamePadBindings.Remove(action);
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Sets a key binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="key">The key.</param>
    public void SetKeyBinding(InputAction action, Keys key) {
        this._keyBindings[action] = key;
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Sets a mouse binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="button">The button.</param>
    public void SetMouseBinding(InputAction action, MouseButton button) {
        this._mouseBindings[action] = button;
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Sets a primary game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="buttons">The buttons.</param>
    public void SetPrimaryGamePadBinding(InputAction action, Buttons buttons) {
        this._primaryGamePadBindings[action] = buttons;
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Sets a secondary game pad binding.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="buttons">The buttons.</param>
    public void SetSecondaryGamePadBinding(InputAction action, Buttons buttons) {
        this._secondaryGamePadBindings[action] = buttons;
        this.BindingChanged.SafeInvoke(this, action);
    }

    /// <summary>
    /// Gets all possible bindings for an action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="primaryGamePadButton">The primary game pad button binding.</param>
    /// <param name="secondaryGamePadButton">The secondary game pad button binding.</param>
    /// <param name="key">The key binding.</param>
    /// <param name="mouseButton">The mouse binding.</param>
    /// <returns>A value indicating whether any of the bindings exist.</returns>
    public bool TryGetBindings(InputAction action, out Buttons primaryGamePadButton, out Buttons secondaryGamePadButton, out Keys key, out MouseButton mouseButton) {
        var result = this._primaryGamePadBindings.TryGetValue(action, out primaryGamePadButton);
        result = this._secondaryGamePadBindings.TryGetValue(action, out secondaryGamePadButton) || result;
        result = this._keyBindings.TryGetValue(action, out key) || result;
        result = this._mouseBindings.TryGetValue(action, out mouseButton) || result;
        return result;
    }
}
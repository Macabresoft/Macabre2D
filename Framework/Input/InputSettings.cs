namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Settings for input.
/// </summary>
[DataContract]
[Category(CommonCategories.Input)]
public class InputSettings {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<InputAction, string> _actionToName = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<string, InputAction> _nameToAction = new();

    /// <summary>
    /// Gets the name of an input action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The name of the action.</returns>
    public string GetName(InputAction action) {
        return this._actionToName.TryGetValue(action, out var name) ? name : string.Empty;
    }

    /// <summary>
    /// Checks whether an action is enabled.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A value indicating whether or not the action is enabled.</returns>
    public bool IsActionEnabled(InputAction action) {
        return this._actionToName.TryGetValue(action, out var name) && !string.IsNullOrEmpty(name);
    }

    /// <summary>
    /// Sets the name of an action if it is not a duplicate
    /// </summary>
    /// <param name="action"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool SetName(InputAction action, string name) {
        var result = false;
        if (string.IsNullOrEmpty(name)) {
            if (this._actionToName.TryGetValue(action, out var originalName)) {
                this._nameToAction.Remove(originalName);
                this._actionToName.Remove(action);
            }
        }
        else if (!this._nameToAction.ContainsKey(name)) {
            if (this._actionToName.TryGetValue(action, out var originalName)) {
                this._nameToAction.Remove(originalName);
            }

            this._actionToName[action] = name;
            this._nameToAction[name] = action;
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Tries to get the specified action by its name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="action">The found action.</param>
    /// <returns>A value indicating the action was found.</returns>
    public bool TryGetAction(string name, out InputAction action) {
        return this._nameToAction.TryGetValue(name, out action);
    }
}
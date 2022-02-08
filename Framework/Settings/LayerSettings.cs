namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Newtonsoft.Json;

/// <summary>
/// Settings for layers.
/// </summary>
[DataContract]
[Category(CommonCategories.Layers)]
public class LayerSettings {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly IDictionary<Layers, string> _layerToName;

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly IDictionary<string, Layers> _nameToLayer;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayerSettings" /> class.
    /// </summary>
    public LayerSettings() {
        this._layerToName = new Dictionary<Layers, string> {
            { Layers.Default, "Default" },
            { Layers.Layer01, "Layer 1" },
            { Layers.Layer02, "Layer 2" },
            { Layers.Layer03, "Layer 3" },
            { Layers.Layer04, "Layer 4" },
            { Layers.Layer05, "Layer 5" },
            { Layers.Layer06, "Layer 6" },
            { Layers.Layer07, "Layer 7" },
            { Layers.Layer08, "Layer 8" },
            { Layers.Layer09, "Layer 9" },
            { Layers.Layer10, "Layer 10" },
            { Layers.Layer11, "Layer 11" },
            { Layers.Layer12, "Layer 12" },
            { Layers.Layer13, "Layer 13" },
            { Layers.Layer14, "Layer 14" },
            { Layers.Layer15, "Layer 15" }
        };

        this._nameToLayer = this._layerToName.ToDictionary(x => x.Value, x => x.Key);
    }

    /// <summary>
    /// Gets a layer from its name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The layer.</returns>
    public Layers GetLayer(string name) {
        return this._nameToLayer.TryGetValue(name, out var layer) ? layer : Layers.None;
    }

    /// <summary>
    /// Gets the name of a layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <returns>The name of a layer.</returns>
    public string GetName(Layers layer) {
        return this._layerToName.TryGetValue(layer, out var name) ? name : layer.GetEnumDisplayName();
    }

    /// <summary>
    /// Sets the name of a layer if it is not a duplicate and the <see cref="layer" /> is a single layer and not multiple.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <param name="name">The name.</param>
    /// <returns>A value indicating whether or not the name was successfully set.</returns>
    public bool SetName(Layers layer, string name) {
        var result = false;

        if (this._layerToName.TryGetValue(layer, out var originalName) && !this._nameToLayer.ContainsKey(name)) {
            this._nameToLayer.Remove(originalName);
            this._layerToName[layer] = name;
            this._nameToLayer[name] = layer;
            result = true;
        }

        return result;
    }
}
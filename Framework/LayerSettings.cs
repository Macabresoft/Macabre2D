using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Macabre2D.Framework {

    [DataContract]
    public sealed class LayerSettings {

        [DataMember]
        private readonly Dictionary<Layers, string> _layersToName = new Dictionary<Layers, string>();

        /// <summary>
        /// Occurs when a layer name has changed.
        /// </summary>
        public event EventHandler<Layers> LayerNameChanged;

        /// <summary>
        /// Gets the name of the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The name of the layer.</returns>
        public string GetLayerName(Layers layer) {
            string name;
            if (!this._layersToName.TryGetValue(layer, out name)) {
                name = layer.ToString();
            }

            return name;
        }

        /// <summary>
        /// Sets the name of the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="name">The name.</param>
        public void SetLayerName(Layers layer, string name) {
            this._layersToName[layer] = name;
            this.LayerNameChanged.SafeInvoke(this, layer);
        }
    }
}
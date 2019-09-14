namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The layer settings.
    /// </summary>
    [DataContract]
    public sealed class LayerSettings {

        [DataMember]
        private readonly Dictionary<Layers, string> _layersToName = new Dictionary<Layers, string>();

        [DataMember]
        private readonly Dictionary<Layers, Layers> _layerToCollisionMask = new Dictionary<Layers, Layers>();

        private List<Layers> _layers;

        public LayerSettings() {
            this._layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().ToList();
            this._layers.Remove(Layers.None);
            this._layers.Remove(Layers.All);
        }

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
        /// Gets a value indicating whether or not the two layers should collide.
        /// </summary>
        /// <param name="firstLayer">The first layer.</param>
        /// <param name="secondLayer">The second layer.</param>
        /// <returns>A value indicating whether or not the two layers should collide.</returns>
        public bool GetShouldCollide(Layers firstLayer, Layers secondLayer) {
            var result = false;
            foreach (var layer in this._layers) {
                if (result) {
                    break;
                }
                else if (firstLayer.HasFlag(layer)) {
                    if (this._layerToCollisionMask.TryGetValue(layer, out var layerMask)) {
                        result = (layerMask & secondLayer) != Layers.None;
                    }
                    else {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the name of the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="name">The name.</param>
        public void SetLayerName(Layers layer, string name) {
            this._layersToName[layer] = name;
        }

        internal void ToggleShouldCollide(Layers rootLayer, Layers collisionBit) {
            if (this._layerToCollisionMask.TryGetValue(rootLayer, out var collisionMask)) {
                if ((collisionMask & collisionBit) == collisionBit) {
                    this._layerToCollisionMask[rootLayer] = collisionMask & (~collisionBit);
                }
                else {
                    this._layerToCollisionMask[rootLayer] = collisionMask | collisionBit;
                }
            }
            else {
                this._layerToCollisionMask[rootLayer] = ~collisionBit;
            }
        }

        internal void ToggleShouldCollide(Layers twoLayers) {
            var rootLayer = Layers.None;
            var collisionBit = Layers.None;

            foreach (var layer in this._layers) {
                if (twoLayers == layer) {
                    rootLayer = layer;
                    collisionBit = layer;
                    break;
                }
                if (twoLayers.HasFlag(layer)) {
                    if (rootLayer == Layers.None) {
                        rootLayer = layer;
                    }
                    else if (collisionBit == Layers.None) {
                        collisionBit = layer;
                        break;
                    }
                }
            }

            if (rootLayer != Layers.None && collisionBit != Layers.None) {
                this.ToggleShouldCollide(rootLayer, collisionBit);
            }
        }
    }
}
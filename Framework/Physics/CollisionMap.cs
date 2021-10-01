namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a collision map of <see cref="Layers" />. Can define on the <see cref="IGamePhysicsSystem" /> level to
    /// determine which layers should collide.
    /// </summary>
    [DataContract]
    public sealed class CollisionMap {
        private readonly List<Layers> _layers;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<Layers, Layers> _layerToCollisionMask = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMap" /> class.
        /// </summary>
        public CollisionMap() {
            this._layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().ToList();
            this._layers.Remove(Layers.None);

            foreach (var layer in this._layers) {
                this._layerToCollisionMask[layer] = ~Layers.None;
            }
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

                if (firstLayer.HasFlag(layer)) {
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
        /// Toggles collisions.
        /// </summary>
        /// <param name="rootLayer">The root layer.</param>
        /// <param name="collisionBit">The collision bit.</param>
        public void ToggleShouldCollide(Layers rootLayer, Layers collisionBit) {
            if (this._layerToCollisionMask.TryGetValue(rootLayer, out var collisionMask)) {
                if ((collisionMask & collisionBit) == collisionBit) {
                    this._layerToCollisionMask[rootLayer] = collisionMask & ~collisionBit;
                }
                else {
                    this._layerToCollisionMask[rootLayer] = collisionMask | collisionBit;
                }
            }
            else {
                this._layerToCollisionMask[rootLayer] = ~collisionBit;
            }
        }

        /// <summary>
        /// Toggles collisions.
        /// </summary>
        /// <param name="twoLayers">The two layers.</param>
        public void ToggleShouldCollide(Layers twoLayers) {
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
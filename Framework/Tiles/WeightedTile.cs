﻿namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A weighted tile that can be used in a <see cref="RandomTileSet"/>.
    /// </summary>
    [DataContract]
    public sealed class WeightedTile {
        private Sprite _sprite;
        private ushort _weight = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedTile"/> class.
        /// </summary>
        internal WeightedTile() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedTile"/> class.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="weight">The weight.</param>
        internal WeightedTile(Sprite sprite, ushort weight) {
            this.Sprite = sprite;
            this.Weight = weight;
        }

        /// <summary>
        /// Occurs when the sprite is changed.
        /// </summary>
        public event EventHandler SpriteChanged;

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public Sprite Sprite {
            get {
                return this._sprite;
            }

            set {
                this._sprite = value;
                this.SpriteChanged.SafeInvoke(this);
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public ushort Weight {
            get {
                return this._weight;
            }

            set {
                this._weight = Math.Max((ushort)1, value);
            }
        }
    }
}
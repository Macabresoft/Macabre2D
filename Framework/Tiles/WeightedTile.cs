namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A weighted tile that can be used in a <see cref="RandomTileSet" />.
    /// </summary>
    [DataContract]
    public sealed class WeightedTile : NotifyPropertyChanged {
        private byte _spriteIndex;
        private ushort _weight = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedTile" /> class.
        /// </summary>
        internal WeightedTile() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedTile" /> class.
        /// </summary>
        /// <param name="spriteIndex">The sprite.</param>
        /// <param name="weight">The weight.</param>
        internal WeightedTile(byte spriteIndex, ushort weight) {
            this.SpriteIndex = spriteIndex;
            this.Weight = weight;
        }

        /// <summary>
        /// Gets or sets the sprite index for this tile.
        /// </summary>
        /// <value>The sprite index.</value>
        [DataMember]
        public byte SpriteIndex {
            get => this._spriteIndex;

            set => this.Set(ref this._spriteIndex, value, true);
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public ushort Weight {
            get => this._weight;

            set => this.Set(ref this._weight, Math.Max((ushort)1, value));
        }
    }
}
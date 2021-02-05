namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// A tile set which automatically provides the correct sprite given its relationship to surrounding tiles.
    /// </summary>
    public sealed class AutoTileSet : SpriteSheetAsset {
        private const byte CardinalSize = 16;
        private const byte IntermediateSize = 48;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<byte, byte> _tileIndexToSpriteIndex = new();

        private bool _useIntermediateDirections;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSet" /> class.
        /// </summary>
        public AutoTileSet() : base() {
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size => this.UseIntermediateDirections ? IntermediateSize : CardinalSize;

        /// <summary>
        /// Gets or sets a value indicating whether this should use intermediate directions when
        /// calculating tiles or just the cardinal directions.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is using intermediate directions in addition to cardinal directions;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "Use Intermediate Directions")]
        public bool UseIntermediateDirections {
            get => this._useIntermediateDirections;

            set {
                if (this.Set(ref this._useIntermediateDirections, value)) {
                    this._useIntermediateDirections = value;
                    this._tileIndexToSpriteIndex.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the sprite at the specified index.
        /// </summary>
        /// <param name="tileIndex">The index.</param>
        /// <returns>The sprite at the specified index.</returns>
        public byte GetSpriteIndex(byte tileIndex) {
            this._tileIndexToSpriteIndex.TryGetValue(tileIndex, out var spriteIndex);
            return spriteIndex;
        }

        /// <summary>
        /// Sets the sprite at the specified index.
        /// </summary>
        /// <param name="spriteIndex">The sprite.</param>
        /// <param name="tileIndex">The index.</param>
        public void SetSprite(byte spriteIndex, byte tileIndex) {
            if (tileIndex < this.Size) {
                this._tileIndexToSpriteIndex[tileIndex] = spriteIndex;
            }
        }

        /// <summary>
        /// Removes the sprite index for the given tile index, effectively blanking it out.
        /// </summary>
        /// <param name="tileIndex">The tile index.</param>
        public void UnsetSprite(byte tileIndex) {
            this._tileIndexToSpriteIndex.Remove(tileIndex);
        }
    }
}
namespace Macabresoft.MonoGame.Core2D {

    using Macabresoft.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for easy and generic implementations of <see cref="IAutoTileSet" />.
    /// </summary>
    public sealed class AutoTileSet : BaseIdentifiable, IAsset {
        private const byte CardinalSize = 16;
        private const byte IntermediateSize = 48;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly Dictionary<byte, Sprite> _indexToSprites = new Dictionary<byte, Sprite>();

        private bool _isLoaded = false;
        private string _name = string.Empty;
        private bool _useIntermediateDirections = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSet" /> class.
        /// </summary>
        public AutoTileSet() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSet" /> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        public AutoTileSet(Guid assetId) : this() {
            this.AssetId = assetId;
        }

        /// <summary>
        /// Occurs when a sprite changes for a particular index.
        /// </summary>
        public event EventHandler<byte>? SpriteChanged;

        /// <inheritdoc />
        [DataMember]
        public Guid AssetId { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size {
            get {
                return this.UseIntermediateDirections ? AutoTileSet.IntermediateSize : AutoTileSet.CardinalSize;
            }
        }

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
            get {
                return this._useIntermediateDirections;
            }

            set {
                if (this.Set(ref this._useIntermediateDirections, value)) {
                    this._useIntermediateDirections = value;
                    this._indexToSprites.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the sprite at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The sprite at the specified index.</returns>
        public Sprite? GetSprite(byte index) {
            this._indexToSprites.TryGetValue(index, out var sprite);
            return sprite;
        }

        /// <summary>
        /// Gets the sprite ids.
        /// </summary>
        /// <returns>The sprite identifiers.</returns>
        public IEnumerable<Guid> GetSpriteIds() {
            return this._indexToSprites.Values.Where(x => x != null).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// Determines whether the specified sprite identifier has sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <returns><c>true</c> if the specified sprite identifier has sprite; otherwise, <c>false</c>.</returns>
        public bool HasSprite(Guid spriteId) {
            return this._indexToSprites.Values.Any(x => x?.Id == spriteId);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load() {
            try {
                foreach (var sprite in this._indexToSprites.Values) {
                    sprite?.Load();
                }
            }
            finally {
                this._isLoaded = true;
            }
        }

        /// <summary>
        /// Refreshes the sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void RefreshSprite(Sprite sprite) {
            if (sprite != null) {
                var indexToSpritesForRefresh = this._indexToSprites.Where(x => x.Value?.Id == sprite.Id).Select(x => new Tuple<byte, Sprite>(x.Key, x.Value)).ToList();
                foreach (var indexToSprite in indexToSpritesForRefresh) {
                    this._indexToSprites[indexToSprite.Item1] = sprite;
                }
            }
        }

        /// <summary>
        /// Removes the sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <returns>A value indicating whether or not the sprite was removed.</returns>
        public bool RemoveSprite(Guid spriteId) {
            var result = false;
            var indexes = this._indexToSprites.Where(x => x.Value?.Id == spriteId).Select(x => x.Key).ToList();

            foreach (var index in indexes) {
                this.SetSprite(null, index);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Sets the sprite at the specified index.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="index">The index.</param>
        public void SetSprite(Sprite? sprite, byte index) {
            if (index < this.Size) {
                if (sprite != null) {
                    this._indexToSprites[index] = sprite;
                    this.SpriteChanged.SafeInvoke(this, index);

                    if (this._isLoaded) {
                        sprite?.Load();
                    }
                }
                else {
                    this._indexToSprites.Remove(index);
                }
            }
        }

        /// <summary>
        /// Tries the get sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <param name="sprite">The sprite.</param>
        /// <returns>A vaue indicating whether or not the sprite was found.</returns>
        public bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this._indexToSprites.Values.FirstOrDefault(x => x?.Id == spriteId);
            return sprite != null;
        }
    }
}
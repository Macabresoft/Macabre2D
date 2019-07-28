namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for easy and generic implementations of <see cref="IAutoTileSet"/>.
    /// </summary>
    public sealed class AutoTileSet : BaseIdentifiable {
        private const byte CardinalSize = 16;
        private const byte IntermediateSize = 48;

        [DataMember]
        private Dictionary<byte, Sprite> _indexToSprites = new Dictionary<byte, Sprite>();

        private bool _isLoaded = false;

        [DataMember]
        private bool _useIntermediateDirections = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSet"/> class.
        /// </summary>
        public AutoTileSet() : base() {
        }

        /// <summary>
        /// Occurs when a sprite changes for a particular index.
        /// </summary>
        public event EventHandler<byte> SpriteChanged;

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
        public bool UseIntermediateDirections {
            get {
                return this._useIntermediateDirections;
            }

            set {
                if (this._useIntermediateDirections != value) {
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
        public Sprite GetSprite(byte index) {
            this._indexToSprites.TryGetValue(index, out var sprite);
            return sprite;
        }

        /// <summary>
        /// Sets the sprite at the specified index.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="index">The index.</param>
        public void SetSprite(Sprite sprite, byte index) {
            if (index < this.Size) {
                if (sprite != null) {
                    this._indexToSprites[index] = sprite;
                    this.SpriteChanged.SafeInvoke(this, index);

                    if (this._isLoaded) {
                        sprite?.LoadContent();
                    }
                }
                else {
                    this._indexToSprites.Remove(index);
                }
            }
        }

        internal IEnumerable<Guid> GetSpriteIds() {
            return this._indexToSprites.Values.Where(x => x != null).Select(x => x.Id);
        }

        internal bool HasSprite(Guid spriteId) {
            return this._indexToSprites.Values.Any(x => x?.Id == spriteId);
        }

        internal void LoadContent() {
            if (!this._isLoaded) {
                try {
                    foreach (var sprite in this._indexToSprites.Values) {
                        sprite?.LoadContent();
                    }
                }
                finally {
                    this._isLoaded = true;
                }
            }
        }

        internal void RefreshSprite(Sprite sprite) {
            if (sprite != null) {
                var indexToSpritesForRefresh = this._indexToSprites.Where(x => x.Value?.Id == sprite.Id).Select(x => (x.Key, x.Value)).ToList();
                foreach (var indexToSprite in indexToSpritesForRefresh) {
                    this._indexToSprites[indexToSprite.Key] = indexToSprite.Value;
                }
            }
        }

        internal bool RemoveSprite(Guid spriteId) {
            var result = false;
            var indexes = this._indexToSprites.Where(x => x.Value?.Id == spriteId).Select(x => x.Key).ToList();

            foreach (var index in indexes) {
                this.SetSprite(null, index);
                result = true;
            }

            return result;
        }

        internal bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this._indexToSprites.Values.FirstOrDefault(x => x?.Id == spriteId);
            return sprite != null;
        }
    }
}
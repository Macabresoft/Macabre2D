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

        private bool _isLoaded = false;

        [DataMember]
        private Sprite[] _sprites = new Sprite[AutoTileSet.CardinalSize];

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
                return this._sprites.Length;
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
                    this.ResetSprites();
                }
            }
        }

        /// <summary>
        /// Gets the sprite at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The sprite at the specified index.</returns>
        public Sprite GetSprite(byte index) {
            Sprite result = null;

            if (index < this._sprites.Length) {
                result = this._sprites[index];
            }

            return result;
        }

        /// <summary>
        /// Sets the sprite at the specified index.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="index">The index.</param>
        public void SetSprite(Sprite sprite, byte index) {
            if (index < this._sprites.Length) {
                this._sprites[index] = sprite;
                this.SpriteChanged.SafeInvoke(this, index);

                if (this._isLoaded) {
                    sprite?.LoadContent();
                }
            }
        }

        internal IEnumerable<Guid> GetSpriteIds() {
            return this._sprites.Where(x => x != null).Select(x => x.Id);
        }

        internal bool HasSprite(Guid spriteId) {
            return this._sprites.Any(x => x?.Id == spriteId);
        }

        internal void LoadContent() {
            if (!this._isLoaded) {
                try {
                    foreach (var sprite in this._sprites) {
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
                for (byte i = 0; i < this._sprites.Length; i++) {
                    if (this._sprites[i] is Sprite oldSprite && oldSprite.Id == sprite.Id) {
                        this._sprites[i] = sprite;
                    }
                }
            }
        }

        internal bool RemoveSprite(Guid spriteId) {
            var result = false;
            for (byte i = 0; i < this._sprites.Length; i++) {
                if (this._sprites[i]?.Id == spriteId) {
                    this.SetSprite(null, i);
                    result = true;
                }
            }

            return result;
        }

        internal bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this._sprites.FirstOrDefault(x => x?.Id == spriteId);
            return sprite != null;
        }

        private void ResetSprites() {
            this._sprites = this.UseIntermediateDirections ? new Sprite[AutoTileSet.IntermediateSize] : new Sprite[AutoTileSet.CardinalSize];
        }
    }
}
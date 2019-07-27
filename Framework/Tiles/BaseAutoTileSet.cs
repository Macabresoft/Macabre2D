namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for easy and generic implementations of <see cref="IAutoTileSet"/>.
    /// </summary>
    public abstract class BaseAutoTileSet : BaseIdentifiable {

        [DataMember]
        private readonly Sprite[] _sprites;

        private bool _isLoaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAutoTileSet"/> class.
        /// </summary>
        public BaseAutoTileSet() : base() {
            this._sprites = new Sprite[this.Size];
        }

        /// <inheritdoc/>
        public event EventHandler<byte> SpriteChanged;

        /// <inheritdoc/>
        public abstract byte Size { get; }

        /// <inheritdoc/>
        public abstract bool UseIntermediateDirections { get; }

        /// <inheritdoc/>
        public Sprite GetSprite(byte index) {
            Sprite result = null;

            if (index < this.Size) {
                result = this._sprites[index];
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetSprite(Sprite sprite, byte index) {
            if (index < this.Size) {
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
    }
}
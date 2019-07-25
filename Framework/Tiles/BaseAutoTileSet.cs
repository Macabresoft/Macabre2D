namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for easy and generic implementations of <see cref="IAutoTileSet"/>.
    /// </summary>
    public abstract class BaseAutoTileSet : BaseIdentifiable, IAutoTileSet {

        [DataMember]
        private readonly Sprite[] _sprites;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAutoTileSet"/> class.
        /// </summary>
        public BaseAutoTileSet() : base() {
            this._sprites = new Sprite[this.Size];
        }

        /// <inheritdoc/>
        public event EventHandler<AutoTileSetSpriteChangedEventArgs> SpriteChanged;

        /// <inheritdoc/>
        public abstract byte Size { get; }

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
                this.SpriteChanged.SafeInvoke(this, new AutoTileSetSpriteChangedEventArgs(index, sprite));
            }
        }
    }
}
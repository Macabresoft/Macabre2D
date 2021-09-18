namespace Macabresoft.Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A step in a sprite animation.
    /// </summary>
    [DataContract]
    public sealed class SpriteAnimationStep : NotifyPropertyChanged {
        private int _frames = 1;
        private byte? _spriteIndex;

        /// <summary>
        /// Gets or sets the number of frames this sprite will be seen.
        /// </summary>
        /// <value>The number of frames.</value>
        [DataMember]
        public int Frames {
            get => this._frames;
            set {
                if (value > 0) {
                    this.Set(ref this._frames, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public byte? SpriteIndex {
            get => this._spriteIndex;
            set => this.Set(ref this._spriteIndex, value);
        }
    }
}
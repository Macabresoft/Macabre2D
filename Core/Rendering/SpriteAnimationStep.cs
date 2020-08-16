namespace Macabresoft.MonoGame.Core {

    using System.Runtime.Serialization;

    /// <summary>
    /// A step in a sprite animation.
    /// </summary>
    [DataContract]
    public sealed class SpriteAnimationStep : NotifyPropertyChanged {
        private int _frames = 1;
        private Sprite _sprite;

        /// <summary>
        /// Gets or sets the number of frames this sprite will be seen.
        /// </summary>
        /// <value>The number of frames.</value>
        [DataMember]
        public int Frames {
            get {
                return this._frames;
            }

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
        public Sprite Sprite {
            get {
                return this._sprite;
            }

            set {
                this.Set(ref this._sprite, value);
            }
        }
    }
}
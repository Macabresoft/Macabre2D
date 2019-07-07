namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A step in a sprite animation.
    /// </summary>
    [DataContract]
    public sealed class SpriteAnimationStep {
        private int _frames = 1;

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
                if (value != this._frames && value > 0) {
                    this._frames = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public Sprite Sprite { get; set; }
    }
}
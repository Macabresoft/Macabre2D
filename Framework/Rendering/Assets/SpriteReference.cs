namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;

    /// <summary>
    /// A reference to a sprite on a <see cref="SpriteSheet" />.
    /// </summary>
    public class SpriteReference : AssetReference<SpriteSheet> {
        /// <summary>
        /// Gets or sets the sprite index on a <see cref="SpriteSheet" />. The sprite sheet is read from left to right, top to
        /// bottom.
        /// </summary>
        [DataMember]
        public byte SpriteIndex { get; set; }
    }
}
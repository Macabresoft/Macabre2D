namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A font to be used by the <see cref="TextRenderer"/>.
    /// </summary>
    public sealed class Font : BaseIdentifiable {

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        public Font() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public Font(Guid id) {
            this.ContentId = id;
            this.LoadSpriteFont();
        }

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        [DataMember]
        public Guid ContentId { get; internal set; }

        internal SpriteFont SpriteFont { get; private set; }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        public void LoadSpriteFont() {
            this.SpriteFont = AssetManager.Instance.Load<SpriteFont>(this.ContentId);
        }
    }
}
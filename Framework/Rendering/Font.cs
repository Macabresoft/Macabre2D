namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A font to be used by the <see cref="TextRenderer"/>.
    /// </summary>
    [DataContract]
    public sealed class Font : IIdentifiable {

        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        public Font() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="contentManager">The content manager.</param>
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

        /// <summary>
        /// Gets the identifier. This is necessary to keep all of the same audio clips in sync.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id {
            get {
                return this._id;
            }

            set {
                this._id = value;
            }
        }

        internal SpriteFont SpriteFont { get; set; }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        public void LoadSpriteFont() {
            this.SpriteFont = AssetManager.Instance.Load<SpriteFont>(this.ContentId);
        }
    }
}
namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework.Content;
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
        public Font(string contentPath, ContentManager contentManager) {
            this.ContentPath = contentPath;
            this.LoadSpriteFont(contentManager);
        }

        /// <summary>
        /// Gets or sets the content path.
        /// </summary>
        /// <value>The content path.</value>
        [DataMember]
        public string ContentPath { get; set; }

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

        /// <summary>
        /// Gets or sets the sprite font.
        /// </summary>
        /// <value>The sprite font.</value>
        internal SpriteFont SpriteFont { get; set; }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public void LoadSpriteFont(ContentManager contentManager) {
            this.SpriteFont = contentManager.Load<SpriteFont>(this.ContentPath);
        }
    }
}
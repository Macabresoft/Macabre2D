namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A font to be used by the <see cref="TextRenderer"/>.
    /// </summary>
    public sealed class Font : BaseIdentifiable, IAsset {

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        public Font() : this(Guid.NewGuid()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        public Font(Guid assetId) {
            this.AssetId = assetId;
        }

        /// <inheritdoc/>
        public Guid AssetId {
            get {
                return this.Id;
            }

            set {
                this.Id = value;
            }
        }

        internal SpriteFont SpriteFont { get; private set; }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        public void Load() {
            this.SpriteFont = AssetManager.Instance.Load<SpriteFont>(this.AssetId);
        }
    }
}
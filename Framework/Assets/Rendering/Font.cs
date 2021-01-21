namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A font to be used by the <see cref="TextRenderComponent" />.
    /// </summary>
    public sealed class Font : BaseIdentifiable, IAsset {

        /// <summary>
        /// Initializes a new instance of the <see cref="Font" /> class.
        /// </summary>
        public Font() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font" /> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        public Font(Guid assetId) {
            this.AssetId = assetId;
        }

        /// <inheritdoc />
        public Guid AssetId {
            get {
                return this.Id;
            }

            set {
                this.Id = value;
            }
        }

        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the sprite font.
        /// </summary>
        /// <value>The sprite font.</value>
        public SpriteFont? SpriteFont { get; private set; }

        /// <summary>
        /// Loads the sound effect.
        /// </summary>
        public void Load() {
            if (AssetManager.Instance.TryLoad<SpriteFont>(this.AssetId, out var spriteFont) && spriteFont != null) {
                this.SpriteFont = spriteFont;
            }
            else {
                this.SpriteFont = null;
            }
        }
    }
}
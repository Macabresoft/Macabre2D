namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an offset in a rectangular area.
    /// </summary>
    public enum PixelOffsetType {
        Bottom = 0,
        BottomLeft = 1,
        BottomRight = 2,
        Center = 3,
        Custom = 4,
        Left = 5,
        Right = 6,
        Top = 7,
        TopLeft = 8,
        TopRight = 9
    }

    /// <summary>
    /// Render settings for rendering a sprite or text. Handles offset and flipping of the sprite.
    /// </summary>
    [DataContract]
    [Category(CommonCategories.Rendering)]
    public sealed class RenderSettings : OffsetSettings {
        private bool _flipHorizontal;
        private bool _flipVertical;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings"/> class.
        /// </summary>
        public RenderSettings() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="type">The type.</param>
        public RenderSettings(Vector2 offset, PixelOffsetType type) : base(offset, type) {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the render should be flipped horizontally.
        /// </summary>
        /// <value><c>true</c> if the render should be flipped horizontally; otherwise, <c>false</c>.</value>
        [DataMember(Name = "Flip Horizontal")]
        public bool FlipHorizontal {
            get {
                return this._flipHorizontal;
            }

            set {
                if (this.Set(ref this._flipHorizontal, value)) {
                    if (this._flipHorizontal) {
                        this.Orientation |= SpriteEffects.FlipHorizontally;
                    }
                    else {
                        this.Orientation &= ~SpriteEffects.FlipHorizontally;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the render should be flipped vertically.
        /// </summary>
        /// <value><c>true</c> if the render should be flipped vertically; otherwise, <c>false</c>.</value>
        [DataMember(Name = "Flip Vertical")]
        public bool FlipVertical {
            get {
                return this._flipVertical;
            }

            set {
                if (this.Set(ref this._flipVertical, value)) {
                    // Since the default state is to flip vertically, this looks a little backwards,
                    // but just trust it!
                    if (this._flipVertical) {
                        this.Orientation &= ~SpriteEffects.FlipVertically;
                    }
                    else {
                        this.Orientation |= SpriteEffects.FlipVertically;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public SpriteEffects Orientation { get; private set; } = SpriteEffects.FlipVertically;
    }
}
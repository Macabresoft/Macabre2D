namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
/// Render options for rendering a sprite or text. Handles offset and flipping of the sprite.
/// </summary>
[DataContract]
[Category(CommonCategories.Rendering)]
public sealed class RenderOptions : OffsetOptions {

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderOptions" /> class.
    /// </summary>
    public RenderOptions() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderOptions" /> class.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="offsetType">The type.</param>
    public RenderOptions(Vector2 offset, PixelOffsetType offsetType) : base(offset, offsetType) {
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    [DataMember]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets a value indicating whether the render should be flipped horizontally.
    /// </summary>
    /// <value><c>true</c> if the render should be flipped horizontally; otherwise, <c>false</c>.</value>
    [DataMember(Name = "Flip Horizontal")]
    public bool FlipHorizontal {
        get;

        set {
            if (value != field) {
                field = value;

                if (field) {
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
        get;

        set {
            if (value != field) {
                field = value;

                // Since the default state is to flip vertically, this looks a little backwards,
                // but just trust it!
                if (field) {
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
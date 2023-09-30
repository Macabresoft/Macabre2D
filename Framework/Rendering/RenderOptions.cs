namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    private bool _flipHorizontal;
    private bool _flipVertical;

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
    /// Gets or sets a value indicating whether the render should be flipped horizontally.
    /// </summary>
    /// <value><c>true</c> if the render should be flipped horizontally; otherwise, <c>false</c>.</value>
    [DataMember(Name = "Flip Horizontal")]
    public bool FlipHorizontal {
        get => this._flipHorizontal;

        set {
            if (value != this._flipHorizontal) {
                this._flipHorizontal = value;

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
        get => this._flipVertical;

        set {
            if (value != this._flipVertical) {
                this._flipVertical = value;

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
    
    /// <summary>
    /// Creates a bounding area for an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The bounding area.</returns>
    public BoundingArea CreateBoundingArea(IEntity entity) {
        BoundingArea result;
        if (this.Size != Vector2.Zero) {
            var unitsPerPixel = entity.Project.UnitsPerPixel;
            var (x, y) = this.Size;
            var width = x * unitsPerPixel;
            var height = y * unitsPerPixel;
            var offset = this.Offset * unitsPerPixel;
            var points = new List<Vector2> {
                entity.GetWorldPosition(offset),
                entity.GetWorldPosition(offset + new Vector2(width, 0f)),
                entity.GetWorldPosition(offset + new Vector2(width, height)),
                entity.GetWorldPosition(offset + new Vector2(0f, height))
            };

            var minimumX = points.Min(point => point.X);
            var minimumY = points.Min(point => point.Y);
            var maximumX = points.Max(point => point.X);
            var maximumY = points.Max(point => point.Y);

            if (entity is IPixelSnappable snappable && snappable.ShouldSnapToPixels(entity.Project)) {
                minimumX = minimumX.ToPixelSnappedValue(entity.Project);
                minimumY = minimumY.ToPixelSnappedValue(entity.Project);
                maximumX = maximumX.ToPixelSnappedValue(entity.Project);
                maximumY = maximumY.ToPixelSnappedValue(entity.Project);
            }

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = BoundingArea.Empty;
        }

        return result;
    }
}
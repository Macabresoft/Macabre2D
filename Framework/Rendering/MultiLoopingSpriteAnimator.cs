namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Renders a sprite repeatedly over an area.
/// </summary>
public class MultiLoopingSpriteAnimator : LoopingSpriteAnimator {
    private int _height = 1;
    private int _width = 1;

    /// <summary>
    /// Gets or sets the height of the area to render this animation. This is how many copies of the animation will be shown, not the number of units.
    /// </summary>
    [DataMember]
    public int Height {
        get => this._height;
        set {
            if (value != this._height) {
                this._height = Math.Max(1, value);
                if (this.IsInitialized) {
                    this.Reset();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the width of the area to render this animation. This is how many copies of the animation will be shown, not the number of units.
    /// </summary>
    [DataMember]
    public int Width {
        get => this._width;
        set {
            if (value != this._width) {
                this._width = Math.Max(1, value);
                if (this.IsInitialized) {
                    this.Reset();
                }
            }
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteIndex.HasValue && this.SpriteBatch is { } spriteBatch && this.SpriteSheet is { } spriteSheet) {
            var xPosition = this.BoundingArea.Minimum.X;
            var yPosition = this.BoundingArea.Minimum.Y;
            var spriteWidth = spriteSheet.SpriteSize.X * this.Project.UnitsPerPixel;
            var spriteHeight = spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            for (var x = 0; x < this.Width; x++) {
                for (var y = 0; y < this.Height; y++) {
                    this.RenderAtPosition(spriteBatch, spriteSheet, this.SpriteIndex.Value, new Vector2(xPosition, yPosition), colorOverride);
                    yPosition += spriteHeight;
                }
                
                yPosition = this.BoundingArea.Minimum.Y;
                xPosition += spriteWidth;
            }
        }
    }

    /// <inheritdoc />
    protected override Vector2 CreateSize() {
        var size = base.CreateSize();
        return new Vector2(size.X * this.Width, size.Y * this.Height);
    }
}
namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Overflow behavior for the <see cref="PixelShiftRenderStep" />.
/// </summary>
public enum OverflowBehavior {
    UseOverflowColor,
    UseSceneBackground,
    WrapAround
}

/// <summary>
/// A render step which shifts pixels. Not up or down, but their position in the linear array.
/// </summary>
public class PixelShiftRenderStep : RenderStep {

    /// <summary>
    /// Gets or sets the amount to shift the pixels.
    /// </summary>
    [DataMember]
    public Point Amount { get; set; }

    /// <summary>
    /// Gets or sets the overflow behavior.
    /// </summary>
    [DataMember]
    public OverflowBehavior Overflow { get; set; } = OverflowBehavior.UseOverflowColor;

    /// <summary>
    /// Gets or sets the overflow color.
    /// </summary>
    /// <remarks>
    /// When the screen shifts to the right, there will be excess space on the left (and vice versa). This excess space will be filled with the specified color.
    /// </remarks>
    [DataMember]
    public Color OverflowColor { get; set; } = Color.Black;

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        if (this.Amount != Point.Zero) {
            var width = previousRenderTarget.Width;
            var count = width * previousRenderTarget.Height;
            var originalData = new Color[count];
            previousRenderTarget.GetData(originalData);
            var newData = new Color[count];
            var hasX = false;

            if (this.Overflow == OverflowBehavior.WrapAround) {
                if (this.Amount.X != 0) {
                    hasX = true;
                    for (var i = 0; i < count; i++) {
                        newData[Math.Abs((i + this.Amount.X) % count)] = originalData[i];
                    }
                }

                if (this.Amount.Y != 0) {
                    if (hasX) {
                        Array.Copy(newData, originalData, count);
                    }

                    var amount = this.Amount.Y * width;
                    for (var i = 0; i < count; i++) {
                        newData[Math.Abs((i + amount) % count)] = originalData[i];
                    }
                }
            }
            else {
                var backgroundColor = this.Overflow == OverflowBehavior.UseSceneBackground ? this.Game.CurrentScene.BackgroundColor : this.OverflowColor;

                if (this.Amount.X > 0) {
                    hasX = true;
                    for (var i = 0; i < this.Amount.X; i++) {
                        newData[i] = backgroundColor;
                    }

                    for (var i = this.Amount.X; i < count; i++) {
                        newData[i] = originalData[i - this.Amount.X];
                    }
                }
                else if (this.Amount.X < 0) {
                    hasX = true;
                    var maxToCopy = count + this.Amount.X;
                    for (var i = 0; i < maxToCopy; i++) {
                        newData[i] = originalData[i - this.Amount.X];
                    }

                    for (var i = maxToCopy; i < count; i++) {
                        newData[i] = backgroundColor;
                    }
                }

                if (this.Amount.Y != 0) {
                    if (hasX) {
                        Array.Copy(newData, originalData, count);
                    }

                    var amount = this.Amount.Y * width;
                    if (this.Amount.Y > 0) {
                        for (var i = 0; i < amount; i++) {
                            newData[i] = backgroundColor;
                        }

                        for (var i = amount; i < count; i++) {
                            newData[i] = originalData[i - amount];
                        }
                    }
                    else {
                        var maxToCopy = count + amount;
                        for (var i = 0; i < maxToCopy; i++) {
                            newData[i] = originalData[i - amount];
                        }

                        for (var i = maxToCopy; i < count; i++) {
                            newData[i] = backgroundColor;
                        }
                    }
                }
            }

            previousRenderTarget.SetData(newData);
        }

        return previousRenderTarget;
    }
}
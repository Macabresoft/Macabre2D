namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A render step which shifts pixels. Not up or down, but their position in the linear array.
/// </summary>
public class PixelShiftRenderStep : RenderStep {

    /// <summary>
    /// Gets or sets the amount to shift the pixels.
    /// </summary>
    [DataMember]
    public Point Amount { get; set; }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        if (this.Amount != Point.Zero) {
            var width = previousRenderTarget.Width;
            var count = width * previousRenderTarget.Height;
            var originalData = new Color[count];
            previousRenderTarget.GetData(originalData);
            var newData = new Color[count];

            var hasX = this.Amount.X != 0;
            if (hasX) {
                // Maybe do amount * width to get vertical shift?
                for (var i = 0; i < count; i++) {
                    newData[Math.Abs((i + this.Amount.X) % count)] = originalData[i];
                }
            }
            
            if (this.Amount.Y != 0) {
                if (hasX) {
                    Array.Copy(newData, originalData, count);
                }

                // Maybe do amount * width to get vertical shift?
                for (var i = 0; i < count; i++) {
                    newData[Math.Abs((i + this.Amount.Y * width) % count)] = originalData[i];
                }
            }
            
            previousRenderTarget.SetData(newData);
        }

        return previousRenderTarget;
    }
} 
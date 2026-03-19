namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A render step which shifts pixels. Not up or down, but their position in the linear array.
/// </summary>
public class PixelShiftRenderStep : ResizeRenderStep {

    /// <summary>
    /// Gets or sets the amount to shift the pixels.
    /// </summary>
    [DataMember]
    public Point Amount { get; set; }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        if (this.Amount != Point.Zero) {
            var renderTarget = this.GetRenderTarget();
            this.Game.GraphicsDevice.SetRenderTarget(renderTarget);
            this.Game.GraphicsDevice.Clear(this.Game.CurrentScene.BackgroundColor);
            spriteBatch.Begin(samplerState: this.SamplerStateType.ToSamplerState());
            spriteBatch.Draw(previousRenderTarget, this.GetAdjustedBounds(previousRenderTarget.Bounds), Color.White);
            spriteBatch.End();
            return renderTarget;
        }

        return previousRenderTarget;
    }

    private Rectangle GetAdjustedBounds(Rectangle originalBounds) {
        var x = originalBounds.X + this.Amount.X;
        var y = originalBounds.Y + this.Amount.Y;
        return new Rectangle(x, y, originalBounds.Width, originalBounds.Height);
    }
}
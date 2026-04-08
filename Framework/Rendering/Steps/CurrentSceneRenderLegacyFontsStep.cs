namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene.
/// </summary>
public class CurrentSceneRenderLegacyFontsStep : RenderStep {

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        var renderSize = new Point(previousRenderTarget.Width, previousRenderTarget.Height);
        this.Game.CurrentScene.RenderLegacyFonts(this.Game.FrameTime, this.Game.InputState, renderSize);
        return previousRenderTarget;
    }
}
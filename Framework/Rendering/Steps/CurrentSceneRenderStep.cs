namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene.
/// </summary>
public class CurrentSceneRenderStep : RenderStep {

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        foreach (var scene in this.Game.OpenScenes.Reverse()) {
            scene.Render(this.Game.FrameTime, this.Game.InputState);
        }

        return previousRenderTarget;
    }
}
namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene.
/// </summary>
public class RenderCurrentScene : RenderStep {

    public override void Initialize(IAssetManager assets, IGame game) {
    }

    public override RenderTarget2D RenderToTexture(
        IGame game,
        GraphicsDevice device,
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution) {
        return previousRenderTarget;
    }

    public override void Reset() {
    }
}
namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class OverlayRenderStep : RenderStep {

    public override void Initialize(IAssetManager assets, IGame game) {
    }

    public override RenderTarget2D RenderToTexture(
        IGame game,
        GraphicsDevice device,
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution) {
        game.Overlay.Render(game.FrameTime, game.InputState);
        return previousRenderTarget;
    }

    public override void Reset() {
    }
}
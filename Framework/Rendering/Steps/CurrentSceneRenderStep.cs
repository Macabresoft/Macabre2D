namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene.
/// </summary>
public class CurrentSceneRenderStep : RenderStep {

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(
        IGame game,
        GraphicsDevice device,
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution) {
        foreach (var scene in game.OpenScenes.Reverse()) {
            scene.Render(game.FrameTime, game.InputState);
        }
        
        return previousRenderTarget;
    }

    /// <inheritdoc />
    public override void Reset() {
    }
}
﻿namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the overlay assigned on <see cref="IGame" />.
/// </summary>
public class OverlayRenderStep : RenderStep {

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution) {
        this.Game.Overlay.Render(this.Game.FrameTime, this.Game.InputState);
        return previousRenderTarget;
    }
}
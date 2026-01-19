namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An instance of a shader that is used by the project as a whole and not specific scenes.
/// </summary>
public class ScreenShaderRenderStep : ResizeRenderStep {
    /// <summary>
    /// Gets the reference to the shader.
    /// </summary>
    [DataMember]
    public ShaderReference Shader { get; } = new();


    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        base.Initialize(assets, game);
        this.Shader.Initialize(assets, game);
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget) {
        if (this.Shader.PrepareAndGetShader(this.RenderSizeFloatingPoint, this.Game, this.Game.CurrentScene) is { } effect) {
            previousRenderTarget = this.RenderToTexture(spriteBatch, previousRenderTarget, effect);
        }

        return previousRenderTarget;
    }
}
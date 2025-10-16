namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Gets the screen shader sizing.
/// </summary>
public enum ScreenShaderSizing {
    /// <summary>
    /// Will use the full size of the graphics device.
    /// </summary>
    FullScreen,

    /// <summary>
    /// Will shrink the render target based on <see cref="IGameProject.PixelsPerUnit" />.
    /// </summary>
    PixelSize,

    /// <summary>
    /// Will render at <see cref="PixelSize" /> with a multiplier until a maximum of <see cref="FullScreen" />.
    /// </summary>
    LimitedPixelSize
}

/// <summary>
/// An instance of a shader that is used by the project as a whole and not specific scenes.
/// </summary>
public class ScreenShaderRenderStep : RenderStep {
    private RenderTarget2D? _renderTarget;

    /// <summary>
    /// Multiplies the render size after applying of <see cref="Sizing" />.
    /// </summary>
    [DataMember]
    public byte Multiplier { get; set; } = 1;

    /// <summary>
    /// Gets or sets the type of the sampler state.
    /// </summary>
    [DataMember(Name = "Sampler State")]
    public SamplerStateType SamplerStateType { get; set; }

    /// <summary>
    /// Gets the reference to the shader.
    /// </summary>
    [DataMember]
    public ShaderReference Shader { get; } = new();

    /// <summary>
    /// Gets the sizing to use when creating a render target.
    /// </summary>
    [DataMember]
    public ScreenShaderSizing Sizing { get; set; } = ScreenShaderSizing.FullScreen;

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        base.Initialize(assets, game);
        this.Shader.Initialize(assets, game);
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution) {
        var renderSize = this.GetRenderSize(viewportSize, internalResolution);
        if (this.Shader.PrepareAndGetShader(renderSize.ToVector2(), this.Game, this.Game.CurrentScene) is { } effect) {
            var renderTarget = this.GetRenderTarget(this.Game.GraphicsDevice, renderSize);
            this.Game.GraphicsDevice.SetRenderTarget(renderTarget);
            this.Game.GraphicsDevice.Clear(this.Game.CurrentScene.BackgroundColor);
            spriteBatch.Begin(effect: effect, samplerState: this.SamplerStateType.ToSamplerState());
            spriteBatch.Draw(previousRenderTarget, renderTarget.Bounds, Color.White);
            spriteBatch.End();
            previousRenderTarget = renderTarget;
        }

        return previousRenderTarget;
    }

    /// <inheritdoc />
    public override void Reset() {
        base.Reset();
        this._renderTarget?.Dispose();
        this._renderTarget = null;
    }

    private Point GetRenderSize(Point viewPortSize, Point pixelRenderSize) {
        return this.Sizing switch {
            ScreenShaderSizing.PixelSize => new Point(pixelRenderSize.X * this.Multiplier, pixelRenderSize.Y * this.Multiplier),
            ScreenShaderSizing.LimitedPixelSize when pixelRenderSize.Y * this.Multiplier is var height && height < viewPortSize.Y => new Point(pixelRenderSize.X * this.Multiplier, height),
            ScreenShaderSizing.LimitedPixelSize => new Point(viewPortSize.X, viewPortSize.Y),
            _ => new Point(viewPortSize.X * this.Multiplier, viewPortSize.Y * this.Multiplier)
        };
    }

    private RenderTarget2D GetRenderTarget(GraphicsDevice device, Point renderSize) {
        this._renderTarget ??= device.CreateRenderTarget(renderSize);
        return this._renderTarget;
    }
}
namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;
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
    private readonly ResettableLazy<Point> _renderSize;
    private readonly ResettableLazy<Vector2> _renderSizeFloatingPoint;
    private bool _isInitialized;
    private RenderTarget2D? _renderTarget;

    public ScreenShaderRenderStep() : base() {
        this._renderSize = new ResettableLazy<Point>(this.GetRenderSize);
        this._renderSizeFloatingPoint = new ResettableLazy<Vector2>(this.GetRenderSizeFloatingPoint);
    }

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
    public ScreenShaderSizing Sizing {
        get;
        set {
            if (value != field) {
                field = value;
                if (this._isInitialized) {
                    this.ResetRenderSize();
                }
            }
        }
    } = ScreenShaderSizing.FullScreen;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._isInitialized = false;
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        base.Initialize(assets, game);
        this.Shader.Initialize(assets, game);
        this._isInitialized = true;
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget) {
        if (this.Shader.PrepareAndGetShader(this._renderSizeFloatingPoint.Value, this.Game, this.Game.CurrentScene) is { } effect) {
            var renderTarget = this.GetRenderTarget(this.Game.GraphicsDevice, this._renderSize.Value);
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

    /// <inheritdoc />
    protected override void OnViewportSizeChanged() {
        base.OnViewportSizeChanged();
        this.ResetRenderSize();
    }

    private Point GetRenderSize() => this.GetRenderSize(this.ViewportSize, this.InternalResolution);

    private Point GetRenderSize(Point viewPortSize, Point pixelRenderSize) {
        return this.Sizing switch {
            ScreenShaderSizing.PixelSize => new Point(pixelRenderSize.X * this.Multiplier, pixelRenderSize.Y * this.Multiplier),
            ScreenShaderSizing.LimitedPixelSize when pixelRenderSize.Y * this.Multiplier is var height && height < viewPortSize.Y => new Point(pixelRenderSize.X * this.Multiplier, height),
            ScreenShaderSizing.LimitedPixelSize => new Point(viewPortSize.X, viewPortSize.Y),
            _ => new Point(viewPortSize.X * this.Multiplier, viewPortSize.Y * this.Multiplier)
        };
    }

    private Vector2 GetRenderSizeFloatingPoint() => this._renderSize.Value.ToVector2();

    private RenderTarget2D GetRenderTarget(GraphicsDevice device, Point renderSize) {
        this._renderTarget ??= device.CreateRenderTarget(renderSize);
        return this._renderTarget;
    }

    private void ResetRenderSize() {
        this._renderSize.Reset();
        this._renderSizeFloatingPoint.Reset();
    }
}
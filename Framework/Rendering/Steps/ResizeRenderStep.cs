namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Gets the screen shader sizing.
/// </summary>
public enum RenderSizing {
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
/// A render step which resizes the current render.
/// </summary>
public class ResizeRenderStep : RenderStep {
    private readonly ResettableLazy<Point> _renderSize;
    private readonly ResettableLazy<Vector2> _renderSizeFloatingPoint;
    private bool _isInitialized;
    private RenderTarget2D? _renderTarget;

    public ResizeRenderStep() : base() {
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
    /// Gets the sizing to use when creating a render target.
    /// </summary>
    [DataMember]
    public RenderSizing Sizing {
        get;
        set {
            if (value != field) {
                field = value;
                if (this._isInitialized) {
                    this.ResetRenderSize();
                }
            }
        }
    } = RenderSizing.FullScreen;

    /// <summary>
    /// Gets the render size.
    /// </summary>
    protected Point RenderSize => this._renderSize.Value;

    /// <summary>
    /// Gets the floating point render size.
    /// </summary>
    protected Vector2 RenderSizeFloatingPoint => this._renderSizeFloatingPoint.Value;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._isInitialized = false;
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        base.Initialize(assets, game);
        this._isInitialized = true;
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) => this.RenderToTexture(spriteBatch, previousRenderTarget, null);

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

    /// <summary>
    /// Renders this step with an optional <see cref="Effect" />.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="previousRenderTarget">The previous render target.</param>
    /// <param name="effect">The effect.</param>
    /// <returns>The render target.</returns>
    protected RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget, Effect? effect) {
        var renderTarget = this.GetRenderTarget();
        this.Game.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Game.GraphicsDevice.Clear(this.Game.CurrentScene.BackgroundColor);
        spriteBatch.Begin(effect: effect, samplerState: this.SamplerStateType.ToSamplerState());
        spriteBatch.Draw(previousRenderTarget, renderTarget.Bounds, Color.White);
        spriteBatch.End();
        return renderTarget;
    }

    private Point GetRenderSize() => this.GetRenderSize(this.ViewportSize, this.InternalResolution);


    private Point GetRenderSize(Point viewPortSize, Point pixelRenderSize) {
        return this.Sizing switch {
            RenderSizing.PixelSize => new Point(pixelRenderSize.X * this.Multiplier, pixelRenderSize.Y * this.Multiplier),
            RenderSizing.LimitedPixelSize when pixelRenderSize.Y * this.Multiplier is var height && height < viewPortSize.Y => new Point(pixelRenderSize.X * this.Multiplier, height),
            RenderSizing.LimitedPixelSize => new Point(viewPortSize.X, viewPortSize.Y),
            _ => new Point(viewPortSize.X * this.Multiplier, viewPortSize.Y * this.Multiplier)
        };
    }

    private Vector2 GetRenderSizeFloatingPoint() => this._renderSize.Value.ToVector2();

    private RenderTarget2D GetRenderTarget() {
        this._renderTarget ??= this.Game.GraphicsDevice.CreateRenderTarget(this.RenderSize);
        return this._renderTarget;
    }

    private void ResetRenderSize() {
        this._renderSize.Reset();
        this._renderSizeFloatingPoint.Reset();
    }
}
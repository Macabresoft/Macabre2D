namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene.
/// </summary>
public class CurrentSceneRenderLegacyFontsStep : RenderStep {
    private readonly ResettableLazy<Point> _renderSize;
    private RenderTarget2D? _renderTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentSceneRenderLegacyFontsStep" /> class.
    /// </summary>
    public CurrentSceneRenderLegacyFontsStep() {
        this._renderSize = new ResettableLazy<Point>(this.GetRenderSize);
    }

    /// <summary>
    /// Gets or sets the type of the sampler state.
    /// </summary>
    [DataMember(Name = "Sampler State")]
    public SamplerStateType SamplerStateType { get; set; }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.Game.ViewportSizeChanged -= this.Game_ViewportSizeChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        base.Initialize(assets, game);
        this.ResetRenderSize();
        this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
    }

    /// <inheritdoc />
    public override RenderTarget2D RenderToTexture(SpriteBatch spriteBatch, RenderTarget2D previousRenderTarget) {
        var renderTarget = this.GetRenderTarget();
        this.Game.GraphicsDevice.SetRenderTarget(renderTarget);
        this.Game.GraphicsDevice.Clear(this.Game.CurrentScene.BackgroundColor);
        spriteBatch.Begin(effect: null, samplerState: this.SamplerStateType.ToSamplerState());
        spriteBatch.Draw(previousRenderTarget, renderTarget.Bounds, Color.White);
        spriteBatch.End();
        
        this.Game.CurrentScene.RenderLegacyFonts(this.Game.FrameTime, this.Game.InputState);
        return renderTarget;
    }

    /// <inheritdoc />
    public override void Reset() {
        base.Reset();
        this._renderSize.Reset();
        this._renderTarget?.Dispose();
        this._renderTarget = null;
    }

    private void Game_ViewportSizeChanged(object? sender, Point e) {
        this.ResetRenderSize();
    }

    private Point GetRenderSize() => this.GetRenderSize(RenderSizing.FullScreen, 1, this.ViewportSize, this.InternalResolution);

    private RenderTarget2D GetRenderTarget() {
        this._renderTarget ??= this.GetRenderTarget(this._renderSize.Value);
        return this._renderTarget;
    }

    private void ResetRenderSize() {
        this._renderSize.Reset();
    }
}
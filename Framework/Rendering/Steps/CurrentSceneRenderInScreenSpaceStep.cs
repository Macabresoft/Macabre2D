namespace Macabre2D.Framework;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Renders the current scene in screen space.
/// </summary>
public class CurrentSceneRenderInScreenSpaceStep : RenderStep {
    private readonly ResettableLazy<Point> _renderSize;
    private RenderTarget2D? _renderTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentSceneRenderInScreenSpaceStep" /> class.
    /// </summary>
    public CurrentSceneRenderInScreenSpaceStep() {
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
        if (this.Game.TryGetGraphicsDevice(out var device) && this.TryGetRenderTarget(out var renderTarget)) {
            device.SetRenderTarget(renderTarget);
            device.Clear(this.Game.CurrentScene.BackgroundColor);
            spriteBatch.Begin(effect: null, samplerState: this.SamplerStateType.ToSamplerState());
            spriteBatch.Draw(previousRenderTarget, renderTarget.Bounds, Color.White);
            spriteBatch.End();

            this.Game.CurrentScene.RenderInScreenSpace(this.Game.FrameTime, this.Game.InputState);
            return renderTarget;
        }

        return previousRenderTarget;
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

    private void ResetRenderSize() {
        this._renderSize.Reset();
    }

    private bool TryGetRenderTarget([NotNullWhen(true)] out RenderTarget2D? renderTarget) {
        if (this._renderTarget == null && this.TryGetRenderTarget(this._renderSize.Value, out renderTarget)) {
            this._renderTarget = renderTarget;
        }
        else {
            renderTarget = this._renderTarget;
        }

        return this._renderTarget != null;
    }
}
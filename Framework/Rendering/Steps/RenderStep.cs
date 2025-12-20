namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a render step.
/// </summary>
public interface IRenderStep : IEnableable, IIdentifiable, INameable {
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="game">The game.</param>
    void Initialize(IAssetManager assets, IGame game);

    /// <summary>
    /// Processes this render step and outputs a new render target.
    /// </summary>
    /// <param name="spriteBatch">The current sprite batch.</param>
    /// <param name="previousRenderTarget">The previous render target.</param>
    /// <param name="viewportSize">The viewport size.</param>
    /// <param name="internalResolution">The internal resolution.</param>
    /// <returns>A new render target, if processing occurred; otherwise, the original render target.</returns>
    RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution);

    /// <summary>
    /// Resets this instance.
    /// </summary>
    /// <remarks>
    /// This is called when view ports and resolutions change, so render steps can throw out their old render targets.
    /// </remarks>
    void Reset();
}

/// <summary>
/// A single
/// </summary>
[DataContract]
public abstract class RenderStep : PropertyChangedNotifier, IRenderStep {

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled {
        get;
        set => this.Set(ref field, value);
    } = true;

    /// <inheritdoc />
    [DataMember]
    public string Name {
        get;
        set => this.Set(ref field, value);
    } = "Render Step";

    /// <summary>
    /// Gets the current <see cref="IGame" />.
    /// </summary>
    protected IGame Game { get; private set; } = BaseGame.Empty;

    /// <inheritdoc />
    public virtual void Initialize(IAssetManager assets, IGame game) {
        this.Game = game;
    }

    /// <inheritdoc />
    public abstract RenderTarget2D RenderToTexture(
        SpriteBatch spriteBatch,
        RenderTarget2D previousRenderTarget,
        Point viewportSize,
        Point internalResolution);

    /// <inheritdoc />
    public virtual void Reset() {
    }
}
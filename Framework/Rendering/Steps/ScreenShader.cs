namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

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
public class ScreenShader : RenderStep {

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
    public override Point GetRenderSize(Point viewPortSize, Point pixelRenderSize) {
        return this.Sizing switch {
            ScreenShaderSizing.PixelSize => new Point(pixelRenderSize.X * this.Multiplier, pixelRenderSize.Y * this.Multiplier),
            ScreenShaderSizing.LimitedPixelSize when pixelRenderSize.Y * this.Multiplier is var height && height < viewPortSize.Y => new Point(pixelRenderSize.X * this.Multiplier, height),
            ScreenShaderSizing.LimitedPixelSize => new Point(viewPortSize.X, viewPortSize.Y),
            _ => new Point(viewPortSize.X * this.Multiplier, viewPortSize.Y * this.Multiplier)
        };
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assets, IGame game) {
        this.Shader.Initialize(assets, game);
    }
}
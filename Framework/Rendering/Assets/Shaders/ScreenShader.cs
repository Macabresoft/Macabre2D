namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
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
public class ScreenShader : PropertyChangedNotifier, IEnableable, IIdentifiable, INameable {
    private bool _isEnabled = true;
    private string _name = "Shader";

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled {
        get => this._isEnabled;
        set => this.Set(ref this._isEnabled, value);
    }

    /// <summary>
    /// Multiplies the render size after applying of <see cref="Sizing" />.
    /// </summary>
    [DataMember]
    public byte Multiplier { get; set; } = 1;

    /// <inheritdoc />
    [DataMember]
    public string Name {
        get => this._name;
        set => this.Set(ref this._name, value);
    }

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

    /// <summary>
    /// Gets the calculated render size based on <see cref="Multiplier" />.
    /// </summary>
    /// <param name="viewPortSize">The view port size.</param>
    /// <param name="pixelRenderSize">The render size in converted pixels.</param>
    /// <returns>The multiplied render size.</returns>
    public Point GetRenderSize(Point viewPortSize, Point pixelRenderSize) {
        return this.Sizing switch {
            ScreenShaderSizing.PixelSize => new Point(pixelRenderSize.X * this.Multiplier, pixelRenderSize.Y * this.Multiplier),
            ScreenShaderSizing.LimitedPixelSize when pixelRenderSize.Y * this.Multiplier is var height && height < viewPortSize.Y => new Point(pixelRenderSize.X * this.Multiplier, height),
            ScreenShaderSizing.LimitedPixelSize => new Point(viewPortSize.X, viewPortSize.Y),
            _ => new Point(viewPortSize.X * this.Multiplier, viewPortSize.Y * this.Multiplier)
        };
    }
}
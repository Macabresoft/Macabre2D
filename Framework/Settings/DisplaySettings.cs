namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// The display mode.
/// </summary>
public enum DisplayMode : byte {
    Borderless = 0,
    Fullscreen = 1,
    Windowed = 2
}

/// <summary>
/// DefaultGraphics settings such as resolution and display mode.
/// </summary>
[DataContract]
[Category(CommonCategories.Display)]
public sealed class DisplaySettings {
    private const byte MinimumWindowScale = 1;

    [DataMember]
    private readonly HashSet<Guid> _disabledScreenShaders = new();

    private byte _windowScale = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplaySettings" /> class.
    /// </summary>
    public DisplaySettings() {
    }

    /// <summary>
    /// Gets or sets the current culture.
    /// </summary>
    [DataMember]
    public ResourceCulture Culture { get; set; } = ResourceCulture.Default;

    /// <summary>
    /// Gets a collection of identifiers for disabled screen shaders.
    /// </summary>
    public IReadOnlyCollection<Guid> DisabledScreenShaders => this._disabledScreenShaders;

    /// <summary>
    /// Gets or sets the display mode.
    /// </summary>
    [DataMember]
    public DisplayMode DisplayMode { get; set; } = DisplayMode.Windowed;

    /// <summary>
    /// Gets or sets the window scale.
    /// </summary>
    [DataMember]
    public byte WindowScale {
        get => this._windowScale;
        set => this._windowScale = Math.Max(MinimumWindowScale, value);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public DisplaySettings Clone() {
        var clone = new DisplaySettings();
        this.CopyTo(clone);
        return clone;
    }

    /// <summary>
    /// Copies the settings to another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    public void CopyTo(DisplaySettings other) {
        other.Culture = this.Culture;
        other.DisplayMode = this.DisplayMode;
        other.WindowScale = this.WindowScale;
        other.EnableAllScreenShaders();
        foreach (var shaderId in this.DisabledScreenShaders) {
            other.DisableScreenShader(shaderId);
        }
    }

    /// <summary>
    /// Disables a screen shader.
    /// </summary>
    /// <param name="identifier">The screen shader identifier.</param>
    public void DisableScreenShader(Guid identifier) {
        this._disabledScreenShaders.Add(identifier);
    }

    /// <summary>
    /// Enables all screen shaders.
    /// </summary>
    public void EnableAllScreenShaders() {
        this._disabledScreenShaders.Clear();
    }

    /// <summary>
    /// Enables a screen shader that was previously disabled.
    /// </summary>
    /// <param name="identifier">The screen shader identifier.</param>
    public void EnableScreenShader(Guid identifier) {
        this._disabledScreenShaders.Remove(identifier);
    }

    /// <summary>
    /// Gets the resolution given the desired number of vertical pixels. This uses the <see cref="AspectRatio" /> and <see cref="WindowScale" /> values.
    /// </summary>
    /// <param name="internalRenderResolution">The internal render resolution.</param>
    /// <returns>The resolution.</returns>
    public Point GetResolution(Point internalRenderResolution) => new(internalRenderResolution.X * this.WindowScale, internalRenderResolution.Y * this.WindowScale);
}
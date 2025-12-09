namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Common;
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
[Category("Display Settings")]
public sealed class DisplaySettings : INameableSettings {
    private const byte MinimumWindowScale = 1;

    [DataMember]
    private readonly HashSet<Guid> _disabledRenderSteps = new();

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
    /// Gets the custom settings.
    /// </summary>
    [DataMember]
    public CustomDisplaySettings CustomSettings { get; } = new();

    /// <summary>
    /// Gets a collection of identifiers for disabled render steps.
    /// </summary>
    public IReadOnlyCollection<Guid> DisabledRenderSteps => this._disabledRenderSteps;

    /// <summary>
    /// Gets or sets the display mode.
    /// </summary>
    [DataMember]
    public DisplayMode DisplayMode { get; set; } = DisplayMode.Windowed;

    /// <inheritdoc />
    public string Name => "Display Settings";

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
        this.CustomSettings.CopyTo(other.CustomSettings);

        other.EnableAllRenderSteps();
        foreach (var shaderId in this.DisabledRenderSteps) {
            other.DisableRenderStep(shaderId);
        }
    }

    /// <summary>
    /// Disables a render step.
    /// </summary>
    /// <param name="identifier">The render step identifier.</param>
    public void DisableRenderStep(Guid identifier) {
        this._disabledRenderSteps.Add(identifier);
    }

    /// <summary>
    /// Enables all render steps.
    /// </summary>
    public void EnableAllRenderSteps() {
        this._disabledRenderSteps.Clear();
    }

    /// <summary>
    /// Enables a render step that was previously disabled.
    /// </summary>
    /// <param name="identifier">The render step identifier.</param>
    public void EnableRenderStep(Guid identifier) {
        this._disabledRenderSteps.Remove(identifier);
    }

    /// <summary>
    /// Gets the resolution given the desired number of vertical pixels.
    /// </summary>
    /// <param name="internalRenderResolution">The internal render resolution.</param>
    /// <returns>The resolution.</returns>
    public Point GetResolution(Point internalRenderResolution) => new(internalRenderResolution.X * this.WindowScale, internalRenderResolution.Y * this.WindowScale);
}
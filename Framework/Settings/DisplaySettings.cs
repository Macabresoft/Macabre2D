namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
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
/// The aspect ratio.
/// </summary>
public enum AspectRatio : byte {
    [Display(Name = "4:3")]
    FourToThree = 0,

    [Display(Name = "16:9")]
    SixteenToNine = 1,

    [Display(Name = "16:10")]
    SixteenToTen = 2,

    [Display(Name = "5:4")]
    FiveToFour = 3,

    [Display(Name = "3:2")]
    ThreeToTwo = 4,

    [Display(Name = "17:9")]
    SeventeenToNine = 5,

    [Display(Name = "21:9")]
    TwentyOneToNine = 6,

    [Display(Name = "32:9")]
    ThirtyTwoToNine = 7,

    [Display(Name = "1:1")]
    OneToOne = 8,

    [Display(Name = "4:1")]
    FourToOne = 9
}

/// <summary>
/// DefaultGraphics settings such as resolution and display mode.
/// </summary>
[DataContract]
[Category(CommonCategories.Display)]
public sealed class DisplaySettings {
    [DataMember]
    private readonly HashSet<Guid> _disabledScreenShaders = new();
    
    private const int DefaultVerticalPixels = 480;
    private const byte MinimumWindowScale = 1;
    private byte _windowScale = 4;


    /// <summary>
    /// Initializes a new instance of the <see cref="DisplaySettings" /> class.
    /// </summary>
    public DisplaySettings() {
    }

    /// <summary>
    /// Gets or sets the aspect ratio.
    /// </summary>
    [DataMember]
    public AspectRatio AspectRatio { get; set; } = AspectRatio.FourToThree;

    /// <summary>
    /// Gets or sets the display mode.
    /// </summary>
    [DataMember]
    public DisplayMode DisplayMode { get; set; } = DisplayMode.Windowed;

    /// <summary>
    /// Gets or sets a value indicating whether or not the persistent overlay should be shown.
    /// </summary>
    [DataMember]
    public bool ShowPersistentOverlay { get; set; } = true;

    /// <summary>
    /// Gets or sets the window scale.
    /// </summary>
    [DataMember]
    public byte WindowScale {
        get => this._windowScale;
        set => this._windowScale = Math.Max(MinimumWindowScale, value);
    }

    /// <summary>
    /// Gets a collection of identifiers for disabled screen shaders.
    /// </summary>
    public IReadOnlyCollection<Guid> DisabledScreenShaders => this._disabledScreenShaders;

    /// <summary>
    /// Enables a screen shader that was previously disabled.
    /// </summary>
    /// <param name="identifier">The screen shader identifier.</param>
    public void EnableScreenShader(Guid identifier) {
        this._disabledScreenShaders.Remove(identifier);
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
        other.DisplayMode = this.DisplayMode;
        other.AspectRatio = this.AspectRatio;
        other.WindowScale = this.WindowScale;
        other.EnableAllScreenShaders();
        foreach (var shaderId in this.DisabledScreenShaders) {
            other.DisableScreenShader(shaderId);
        }
    }

    /// <summary>
    /// Gets the display name for a ratio.
    /// </summary>
    /// <param name="ratio">The ratio.</param>
    /// <returns>The display name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The ratio requested has not been accounted for.</exception>
    public static string GetRatioDisplayName(AspectRatio ratio) {
        return ratio switch {
            AspectRatio.FourToThree => "4:3",
            AspectRatio.SixteenToNine => "16:9",
            AspectRatio.SixteenToTen => "16:10",
            AspectRatio.FiveToFour => "5:4",
            AspectRatio.ThreeToTwo => "3:2",
            AspectRatio.SeventeenToNine => "17:9",
            AspectRatio.TwentyOneToNine => "21:9",
            AspectRatio.ThirtyTwoToNine => "32:9",
            AspectRatio.OneToOne => "1:1",
            AspectRatio.FourToOne => "4:1",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Gets the resolution given the desired number of vertical pixels. This uses the <see cref="AspectRatio" /> and <see cref="WindowScale" /> values.
    /// </summary>
    /// <param name="verticalPixels">The number of vertical pixels desired on screen.</param>
    /// <returns>The resolution.</returns>
    public Point GetResolution(int verticalPixels) {
        if (verticalPixels <= 0) {
            verticalPixels = DefaultVerticalPixels;
        }

        var resolutionY = verticalPixels * this.WindowScale;
        var resolutionX = (int)Math.Ceiling(resolutionY * this.GetRatio());

        return new Point(resolutionX, resolutionY);
    }

    private float GetRatio() {
        return this.AspectRatio switch {
            AspectRatio.FourToThree => 4f / 3f,
            AspectRatio.SixteenToNine => 16f / 9f,
            AspectRatio.SixteenToTen => 16f / 10f,
            AspectRatio.FiveToFour => 5f / 4f,
            AspectRatio.ThreeToTwo => 3f / 2f,
            AspectRatio.SeventeenToNine => 17f / 9f,
            AspectRatio.TwentyOneToNine => 21 / 9f,
            AspectRatio.ThirtyTwoToNine => 32 / 9f,
            AspectRatio.OneToOne => 1f,
            AspectRatio.FourToOne => 4f,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
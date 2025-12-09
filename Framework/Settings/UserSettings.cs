namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// User customizable settings.
/// </summary>
[DataContract]
[Category("User Settings")]
public class UserSettings : CopyableSettings<UserSettings> {
    /// <summary>
    /// Gets the types of the various settings.
    /// </summary>
    public static readonly List<Type> UserSettingsTypes = [
        typeof(UserSettings),
        typeof(AudioSettings),
        typeof(GameplaySettings),
        typeof(DisplaySettings),
        typeof(InputSettings),
        typeof(RenderSettings)
    ];
    
    /// <summary>
    /// The settings file name.
    /// </summary>
    public const string FileName = "UserSettings.m2d";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    public UserSettings() : this(new AudioSettings(), new RenderSettings(), new DisplaySettings(), new InputSettings(), new GameplaySettings()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="project">The game project.</param>
    public UserSettings(IGameProject project) {
        this.Audio = project.DefaultUserSettings.Audio.Clone();
        this.Rendering = project.DefaultUserSettings.Rendering.Clone();
        this.Display = project.DefaultUserSettings.Display.Clone();
        this.Input = project.DefaultUserSettings.Input.Clone();
        this.Gameplay = project.DefaultUserSettings.Gameplay;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="rendering">The color settings.</param>
    /// <param name="display">The graphics settings.</param>
    /// <param name="inputSettings">The input bindings.</param>
    /// <param name="gameplay">The custom settings.</param>
    public UserSettings(AudioSettings audio, RenderSettings rendering, DisplaySettings display, InputSettings inputSettings, GameplaySettings gameplay) {
        this.Audio = audio;
        this.Rendering = rendering;
        this.Display = display;
        this.Input = inputSettings;
        this.Gameplay = gameplay;
    }

    /// <summary>
    /// Gets the audio settings.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Audio)]
    public AudioSettings Audio { get; }

    /// <summary>
    /// Gets the custom user settings.
    /// </summary>
    [DataMember]
    public GameplaySettings Gameplay { get; }

    /// <summary>
    /// Gets the graphics settings.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Display)]
    public DisplaySettings Display { get; }

    /// <summary>
    /// Gets the input bindings.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Input)]
    public InputSettings Input { get; }

    /// <summary>
    /// Gets the color settings.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderSettings Rendering { get; }

    /// <inheritdoc />
    public override UserSettings Clone() => new(this.Audio, this.Rendering, this.Display, this.Input, this.Gameplay);

    /// <inheritdoc />
    public override void CopyTo(UserSettings other) {
        base.CopyTo(other);

        this.Audio.CopyTo(other.Audio);
        this.Rendering.CopyTo(other.Rendering);
        this.Display.CopyTo(other.Display);
        this.Input.CopyTo(other.Input);
        this.Gameplay.CopyTo(other.Gameplay);
    }
}
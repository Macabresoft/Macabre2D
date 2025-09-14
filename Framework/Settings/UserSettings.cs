namespace Macabresoft.Macabre2D.Framework;

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
    /// The settings file name.
    /// </summary>
    public const string FileName = "UserSettings.m2d";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    public UserSettings() : this(new AudioSettings(), new RenderSettings(), new DisplaySettings(), new InputSettings(), new CustomSettings()) {
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
        this.Custom = project.DefaultUserSettings.Custom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="rendering">The color settings.</param>
    /// <param name="display">The graphics settings.</param>
    /// <param name="inputSettings">The input bindings.</param>
    /// <param name="custom">The custom settings.</param>
    public UserSettings(AudioSettings audio, RenderSettings rendering, DisplaySettings display, InputSettings inputSettings, CustomSettings custom) {
        this.Audio = audio;
        this.Rendering = rendering;
        this.Display = display;
        this.Input = inputSettings;
        this.Custom = custom;
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
    public CustomSettings Custom { get; }

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
    public override UserSettings Clone() => new(this.Audio, this.Rendering, this.Display, this.Input, this.Custom);

    /// <inheritdoc />
    public override void CopyTo(UserSettings settings) {
        this.Audio.CopyTo(settings.Audio);
        this.Rendering.CopyTo(settings.Rendering);
        this.Display.CopyTo(settings.Display);
        this.Input.CopyTo(settings.Input);
        this.Custom.CopyTo(settings.Custom);
    }
}
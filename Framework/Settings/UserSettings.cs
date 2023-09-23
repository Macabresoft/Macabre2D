namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// User customizable settings.
/// </summary>
[DataContract]
[Category("User Settings")]
public class UserSettings : VersionedData {
    /// <summary>
    /// The settings file name.
    /// </summary>
    public const string FileName = "UserSettings.m2d";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    public UserSettings() : this(new AudioSettings(), new DisplaySettings(), new InputBindings(), new CustomSettings()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="project">The game project.</param>
    public UserSettings(IGameProject project) {
        this.Audio = project.DefaultUserSettings.Audio.Clone();
        this.Display = project.DefaultUserSettings.Display.Clone();
        this.Input = project.DefaultUserSettings.Input.Clone();
        this.CustomSettings = project.DefaultUserSettings.CustomSettings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="display">The graphics settings.</param>
    /// <param name="inputBindings">The input bindings.</param>
    /// <param name="customSettings">The custom settings.</param>
    public UserSettings(AudioSettings audio, DisplaySettings display, InputBindings inputBindings, CustomSettings customSettings) {
        this.Audio = audio;
        this.Display = display;
        this.Input = inputBindings;
        this.CustomSettings = customSettings;
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
    public CustomSettings CustomSettings { get; }

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
    public InputBindings Input { get; }
}
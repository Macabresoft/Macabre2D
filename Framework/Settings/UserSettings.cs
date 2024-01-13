namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Common;
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
        this.Custom = project.DefaultUserSettings.Custom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="display">The graphics settings.</param>
    /// <param name="inputBindings">The input bindings.</param>
    /// <param name="custom">The custom settings.</param>
    public UserSettings(AudioSettings audio, DisplaySettings display, InputBindings inputBindings, CustomSettings custom) {
        this.Audio = audio;
        this.Display = display;
        this.Input = inputBindings;
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
    public InputBindings Input { get; }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public UserSettings Clone() {
        return new UserSettings(this.Audio, this.Display, this.Input, this.Custom);
    }

    /// <summary>
    /// Copies settings to another instance.
    /// </summary>
    /// <param name="settings">The other instance.</param>
    public void CopyTo(UserSettings settings) {
        this.Audio.CopyTo(settings.Audio);
        this.Display.CopyTo(settings.Display);
        this.Input.CopyTo(settings.Input);
        this.Custom.CopyTo(settings.Custom);
    }
}
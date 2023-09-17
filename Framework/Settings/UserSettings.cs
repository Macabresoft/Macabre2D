namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

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
    public UserSettings() : this(new AudioSettings(), new DisplaySettings(), new InputBindings()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="settings">The game settings.</param>
    public UserSettings(IGameSettings settings) {
        this.Audio = settings.DefaultUserSettings.Audio.Clone();
        this.Display = settings.DefaultUserSettings.Display.Clone();
        this.Input = settings.DefaultUserSettings.Input.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettings" /> class.
    /// </summary>
    /// <param name="audio">The audio settings.</param>
    /// <param name="display">The graphics settings.</param>
    /// <param name="inputBindings">The input bindings.</param>
    public UserSettings(AudioSettings audio, DisplaySettings display, InputBindings inputBindings) {
        this.Audio = audio;
        this.Display = display;
        this.Input = inputBindings;
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
    public NamedSettingCollection CustomSettings { get; } = new();

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
namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework.UserSettingInstances;

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

    [DataMember]
    private readonly List<NamedSetting> _customSettings = new();

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
        this.Audio = settings.DefaultAudioSettings.Clone();
        this.Display = settings.DefaultDisplaySettings.Clone();
        this.InputBindings = settings.InputSettings.DefaultBindings.Clone();
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
        this.InputBindings = inputBindings;
    }

    /// <summary>
    /// Gets the audio settings.
    /// </summary>
    public AudioSettings Audio { get; }

    /// <summary>
    /// Gets the graphics settings.
    /// </summary>
    public DisplaySettings Display { get; }

    /// <summary>
    /// Gets the input bindings.
    /// </summary>
    public InputBindings InputBindings { get; }

    /// <summary>
    /// Adds a setting.
    /// </summary>
    /// <param name="setting">The setting to add.</param>
    public void AddSetting(NamedSetting setting) {
        this._customSettings.Add(setting);
    }

    /// <summary>
    /// Removes a setting by name.
    /// </summary>
    /// <param name="settingName">The setting's name.</param>
    public void RemoveSetting(string settingName) {
        this._customSettings.RemoveAll(x => x.Name == settingName);
    }

    /// <summary>
    /// Removes a setting by name.
    /// </summary>
    /// <param name="setting">The setting.</param>
    public void RemoveSetting(NamedSetting setting) {
        this._customSettings.Remove(setting);
    }

    /// <summary>
    /// Attempts to get a <see cref="bool" /> setting if possible.
    /// </summary>
    /// <param name="settingName">The setting's name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether or not the setting was found.</returns>
    public bool TryGetSetting<T>(string settingName, [NotNullWhen(true)] out T? value) where T : NamedSetting {
        value = this._customSettings.OfType<T>().FirstOrDefault(x => x.Name == settingName);
        return value != null;
    }
}
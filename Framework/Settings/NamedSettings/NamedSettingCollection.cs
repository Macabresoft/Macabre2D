namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A collection of <see cref="NamedSetting" />.
/// </summary>
[DataContract]
public class NamedSettingCollection : IReadOnlyCollection<NamedSetting> {
    [DataMember]
    private readonly List<NamedSetting> _customSettings = new();

    /// <inheritdoc />
    public int Count => this._customSettings.Count;

    /// <summary>
    /// Adds a setting.
    /// </summary>
    /// <param name="setting">The setting to add.</param>
    public void AddSetting(NamedSetting setting) {
        this._customSettings.Add(setting);
    }

    /// <inheritdoc />
    public IEnumerator<NamedSetting> GetEnumerator() {
        return this._customSettings.GetEnumerator();
    }

    /// <summary>
    /// Removes a setting by name.
    /// </summary>
    /// <param name="setting">The setting.</param>
    public void RemoveSetting(NamedSetting setting) {
        this._customSettings.Remove(setting);
    }

    /// <summary>
    /// Replaces one setting with another.
    /// </summary>
    /// <param name="existing">The existing setting.</param>
    /// <param name="replacement">The replacement.</param>
    public void Replace(NamedSetting existing, NamedSetting replacement) {
        var index = this._customSettings.IndexOf(existing);
        if (index >= 0) {
            this._customSettings.Remove(existing);
            this._customSettings.Insert(index, replacement);
        }
    }

    /// <summary>
    /// Resets this collection with new values.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void Reset(IEnumerable<NamedSetting> settings) {
        this._customSettings.Clear();
        this._customSettings.AddRange(settings);
    }

    /// <summary>
    /// Attempts to get a setting by name.
    /// </summary>
    /// <param name="settingName">The setting's name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether or not the setting was found.</returns>
    public bool TryGetSetting<T>(string settingName, [NotNullWhen(true)] out T? value) where T : NamedSetting {
        value = this._customSettings.OfType<T>().FirstOrDefault(x => x.Name == settingName);
        return value != null;
    }

    /// <summary>
    /// Attempts to get a setting by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether or not the setting was found.</returns>
    /// <returns></returns>
    public bool TryGetSetting<T>(Guid id, [NotNullWhen(true)] out T? value) where T : NamedSetting {
        value = this._customSettings.OfType<T>().FirstOrDefault(x => x.Id == id);
        return value != null;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}
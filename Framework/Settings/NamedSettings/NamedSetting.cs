namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Base class for instances of specific settings found in <see cref="UserSettings" />.
/// </summary>
[DataContract]
public abstract class NamedSetting : IIdentifiable, INameable  {
    /// <summary>
    /// Gets or sets the category to which this setting belongs.
    /// </summary>
    [DataMember]
    public SettingsCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the description of this setting. Can be used in game or simply for a developer's convenience.
    /// </summary>
    [DataMember]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of this setting. This is also used as the key when looking for this setting.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of this setting.
    /// </summary>
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();
}
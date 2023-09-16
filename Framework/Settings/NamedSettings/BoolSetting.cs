namespace Macabresoft.Macabre2D.Framework.UserSettingInstances;

using System.Runtime.Serialization;

/// <summary>
/// A <see cref="NamedSetting" /> for <see cref="bool" /> values.
/// </summary>
public class BoolSetting : NamedSetting {
    /// <summary>
    /// Gets or sets the value of this setting.
    /// </summary>
    [DataMember]
    public bool Value { get; set; }
}
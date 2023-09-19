namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// A <see cref="NamedSetting" /> for <see cref="bool" /> values.
/// </summary>
public class BoolSetting : NamedSetting {
    /// <inheritdoc />
    public override Type ValueType => typeof(bool);

    /// <summary>
    /// Gets or sets the value of this setting.
    /// </summary>
    [DataMember]
    public bool Value { get; set; }
}
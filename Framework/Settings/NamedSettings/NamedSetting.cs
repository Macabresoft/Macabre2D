namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Base class for instances of specific settings found in <see cref="UserSettings" />.
/// </summary>
[DataContract]
public abstract class NamedSetting : IIdentifiable, INameable {
    /// <summary>
    /// Gets the of the value.
    /// </summary>
    public abstract Type ValueType { get; }
    
    /// <summary>
    /// Gets the untyped value of this setting.
    /// </summary>
    public abstract object UntypedValue { get; }

    /// <summary>
    /// Gets or sets the category to which this setting belongs.
    /// </summary>
    [DataMember]
    public SettingsCategory Category { get; set; } = SettingsCategory.Miscellaneous;

    /// <summary>
    /// Gets or sets the identifier of this setting.
    /// </summary>
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of this setting. This is also used as the key when looking for this setting.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Copies information from another <see cref="NamedSetting" />.
    /// </summary>
    /// <param name="other">The other value.</param>
    public virtual void CopyInformationFromOther(NamedSetting other) {
        this.Category = other.Category;
        this.Name = other.Name;
    }
}
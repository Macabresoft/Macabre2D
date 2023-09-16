namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>
/// An instance of a user setting.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
[DataContract]
public class UserSettingInstance<T> where T: notnull {

    
    
    /// <summary>
    /// Gets or sets value of this user setting.
    /// </summary>
    [DataMember]
    public T Value { get; set; }
    
    /// <summary>
    /// Gets or sets the name of this setting. This is also used as the key when looking for this setting.
    /// </summary>
    public string Name { get; set; }
}
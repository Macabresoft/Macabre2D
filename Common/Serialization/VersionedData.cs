namespace Macabresoft.Macabre2D.Common;

using System.Runtime.Serialization;

/// <summary>
/// Interface for data that can be versioned.
/// </summary>
public interface IVersionedData {
    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    /// <value>The name of the type.</value>
    [DataMember]
    string TypeName { get; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    [DataMember]
    Version Version { get; set; }
}

/// <summary>
/// Data that can be versioned.
/// </summary>
[DataContract]
public class VersionedData : IVersionedData {
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionedData" /> class.
    /// </summary>
    /// <param name="version">The version.</param>
    public VersionedData(Version version) : this() {
        this.Version = version;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionedData" /> class.
    /// </summary>
    protected VersionedData() {
        this.TypeName = this.GetType().Name;
    }

    /// <inheritdoc />
    /// <remarks>The setter here is required for this to actually get loaded with <see cref="DataMemberAttribute"/>.</remarks>
    [DataMember]
    public string TypeName { get; private set; }

    /// <inheritdoc />
    [DataMember]
    public Version Version { get; set; } = new();
}
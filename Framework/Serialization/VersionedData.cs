namespace Macabresoft.Macabre2D.Framework {

    using System;
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
        [DataMember]
        public string TypeName { get; private set; } = string.Empty;

        /// <inheritdoc />
        [DataMember]
        public Version Version { get; set; } = new Version();
    }
}
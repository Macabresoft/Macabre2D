namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Data that can be versioned.
    /// </summary>
    [DataContract]
    public class VersionedData {

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedData"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public VersionedData(Version version) : this() {
            this.Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedData"/> class.
        /// </summary>
        protected VersionedData() {
            this.TypeName = this.GetType().Name;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [DataMember]
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DataMember]
        public Version Version { get; set; }
    }
}
namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base implementation of <see cref="IIdentifiable"/>.
    /// </summary>
    [DataContract]
    public class BaseIdentifiable : IIdentifiable {

        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }

            internal set {
                this._id = value;
            }
        }
    }
}
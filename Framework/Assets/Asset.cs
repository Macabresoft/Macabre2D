namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an object that is an asset.
    /// </summary>
    public interface IAsset : INotifyPropertyChanged {

        /// <summary>
        /// Gets the asset identifier.
        /// </summary>
        /// <value>The asset identifier.</value>
        Guid AssetId { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
    
    /// <summary>
    /// A base implementation for assets that contains an identifier and name.
    /// </summary>
    [DataContract]
    public abstract class Asset : NotifyPropertyChanged, IAsset {
        private string _name = string.Empty;

        /// <inheritdoc />
        [DataMember]
        public Guid AssetId { get; } = Guid.NewGuid();

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }
    }
}
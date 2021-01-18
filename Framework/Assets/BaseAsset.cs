namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base implementation for assets that contains an identifier and name.
    /// </summary>
    [DataContract]
    public class BaseAsset : NotifyPropertyChanged, IAsset {
        private string _name = string.Empty;

        /// <inheritdoc />
        [DataMember]
        public Guid AssetId { get; set; }

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
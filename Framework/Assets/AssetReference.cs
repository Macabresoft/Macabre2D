namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A reference to an asset using identifier and type. Held by components for serialization.
    /// </summary>
    /// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
    [DataContract]
    public class AssetReference<TAsset> : NotifyPropertyChanged, IDisposable where TAsset : class, IAsset {
        private Guid _assetId;

        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        public Type Type => typeof(TAsset);

        /// <summary>
        /// Gets the asset.
        /// </summary>
        public TAsset? Asset { get; protected set; }

        /// <summary>
        /// Gets or sets the asset identifier.
        /// </summary>
        [DataMember]
        public Guid AssetId {
            get => this._assetId;
            set {
                this._assetId = value;
                this.OnAssetIdChanged();
            }
        }

        /// <inheritdoc />
        public void Dispose() {
            if (this.Asset != null) {
                this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
            }
        }

        /// <summary>
        /// Initializes this instance with an asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        public virtual void Initialize(TAsset asset) {
            if (this.Asset != null) {
                this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
            }

            this.Asset = asset;
            this.Asset.PropertyChanged += this.Asset_PropertyChanged;
            this._assetId = this.Asset.AssetId;
        }

        /// <summary>
        /// Occurs when the asset identifier changes for this reference.
        /// </summary>
        protected virtual void OnAssetIdChanged() {
            if (this.Asset != null && this.Asset.AssetId != this._assetId) {
                this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
            }
        }
        
        private void Asset_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender ?? this, e);
        }
    }
}
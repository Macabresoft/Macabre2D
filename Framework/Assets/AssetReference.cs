namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// A reference to an asset using identifier and type. Held by entities for serialization.
    /// </summary>
    /// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
    [DataContract]
    public class AssetReference<TAsset> : NotifyPropertyChanged where TAsset : class, IAsset {
        /// <summary>
        /// Gets the asset.
        /// </summary>
        public TAsset? Asset { get; private set; }

        /// <summary>
        /// Gets or sets the asset identifier.
        /// </summary>
        [DataMember]
        public Guid ContentId { get; private set; }

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
            this.ContentId = this.Asset.ContentId;
        }

        /// <inheritdoc />
        protected override void OnDisposing() {
            base.OnDisposing();

            if (this.Asset != null) {
                this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender ?? this, e);
        }
    }
}
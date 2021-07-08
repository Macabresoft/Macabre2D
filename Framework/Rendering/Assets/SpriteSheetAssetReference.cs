namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// An asset reference for an asset packaged inside of a <see cref="SpriteSheet" />.
    /// </summary>
    /// <typeparam name="TPackagedAsset">The type of packaged asset.</typeparam>
    public class SpriteSheetAssetReference<TPackagedAsset> : AssetReference<SpriteSheet> where TPackagedAsset : SpriteSheetAsset {
        private Guid _packagedAssetId;

        /// <summary>
        /// Gets the packaged asset.
        /// </summary>
        public TPackagedAsset? PackagedAsset { get; private set; }

        /// <summary>
        /// Gets the packaged asset identifier.
        /// </summary>
        [DataMember]
        public Guid PackagedAssetId {
            get => this._packagedAssetId;
            set {
                if (this.Set(ref this._packagedAssetId, value)) {
                    this.ResolvePackagedAsset();
                }
            }
        }

        /// <inheritdoc />
        public override void Initialize(SpriteSheet asset) {
            base.Initialize(asset);

            if (this.PackagedAsset is TPackagedAsset originalPackagedAsset) {
                originalPackagedAsset.PropertyChanged -= this.SpriteSheet_OnPropertyChanged;
            }

            this.ResolvePackagedAsset();

            if (this.PackagedAsset is TPackagedAsset newPackagedAsset) {
                newPackagedAsset.PropertyChanged += this.SpriteSheet_OnPropertyChanged;
            }
        }

        private void ResolvePackagedAsset() {
            if (this.Asset != null && this.Asset.TryGetPackaged<TPackagedAsset>(this.PackagedAssetId, out var packagedAsset)) {
                this.PackagedAsset = packagedAsset;
            }
        }

        private void SpriteSheet_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender, e);
        }
    }
}
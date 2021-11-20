namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// An asset reference for an asset packaged inside of a <see cref="SpriteSheet" />.
    /// </summary>
    /// <typeparam name="TPackagedAsset">The type of packaged asset.</typeparam>
    public class SpriteSheetAssetReference<TPackagedAsset> : AssetReference<SpriteSheet> where TPackagedAsset : SpriteSheetAsset {
        private TPackagedAsset? _packagedAsset;
        private Guid _packagedAssetId;

        /// <summary>
        /// Gets the packaged asset.
        /// </summary>
        public TPackagedAsset? PackagedAsset {
            get => this._packagedAsset;
            private set {
                var originalAsset = this._packagedAsset;

                if (this.Set(ref this._packagedAsset, value)) {
                    if (originalAsset != null) {
                        originalAsset.PropertyChanged -= this.SpriteSheet_OnPropertyChanged;
                    }

                    if (this._packagedAsset != null) {
                        this._packagedAsset.PropertyChanged += this.SpriteSheet_OnPropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the packaged asset identifier.
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
        public override void Clear() {
            base.Clear();
            this.PackagedAsset = null;
            this._packagedAssetId = Guid.Empty;
        }

        /// <inheritdoc />
        public override void Initialize(SpriteSheet asset) {
            base.Initialize(asset);
            this.ResolvePackagedAsset();
        }

        private void ResolvePackagedAsset() {
            if (this.PackagedAssetId != Guid.Empty && this.Asset != null && this.Asset.TryGetPackaged<TPackagedAsset>(this.PackagedAssetId, out var packagedAsset)) {
                this.PackagedAsset = packagedAsset;
            }
            else {
                this.PackagedAsset = null;
            }
        }

        private void SpriteSheet_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender, e);
        }
    }
}
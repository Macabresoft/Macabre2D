namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A packaged asset reference.
    /// </summary>
    /// <typeparam name="TAssetPackage">The asset package.</typeparam>
    /// <typeparam name="TPackagedAsset">The packaged asset.</typeparam>
    public class PackagedAssetReference<TAssetPackage, TPackagedAsset> : AssetReference<TPackagedAsset>
        where TAssetPackage : class, IAsset, IAssetPackage
        where TPackagedAsset : class, IAsset, IPackagedAsset<TAssetPackage> {
        /// <summary>
        /// Gets or sets the asset identifier.
        /// </summary>
        [DataMember]
        public Guid PackageId { get; private set; }

        /// <inheritdoc />
        public override void Initialize(TPackagedAsset asset) {
            base.Initialize(asset);
            this.PackageId = asset.PackageId;
        }
    }
}
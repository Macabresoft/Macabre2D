namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base asset for assets which are packaged in a <see cref="SpriteSheet" />.
    /// </summary>
    public class SpriteSheetAsset : Asset, IPackagedAsset<SpriteSheet> {
        /// <inheritdoc />
        [DataMember]
        public Guid PackageId { get; private set; }

        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet? SpriteSheet { get; private set; }

        /// <inheritdoc />
        public void Initialize(SpriteSheet owningPackage) {
            this.SpriteSheet = owningPackage;
            this.PackageId = this.SpriteSheet.AssetId;
        }
    }
}
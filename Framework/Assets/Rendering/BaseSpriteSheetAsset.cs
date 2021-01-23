namespace Macabresoft.Macabre2D.Framework {
    /// <summary>
    /// A base asset for assets which are packaged in a <see cref="SpriteSheet"/>.
    /// </summary>
    public class BaseSpriteSheetAsset : BaseAsset, IPackagedAsset<SpriteSheet> {
        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet? SpriteSheet { get; private set; }
        
        /// <inheritdoc />
        public void Initialize(SpriteSheet owningPackage) {
            this.SpriteSheet = owningPackage;
        }
    }
}
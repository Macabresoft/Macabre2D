namespace Macabresoft.Macabre2D.Framework {
    using System;

    /// <summary>
    /// An asset reference for an asset packaged inside of a <see cref="SpriteSheet"/>.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    public class SpriteSheetAssetReference<TAsset> : AssetReference<TAsset> where TAsset : BaseSpriteSheetAsset {
        
        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet? SpriteSheet => this.Asset?.SpriteSheet;

        /// <inheritdoc />
        protected override void OnAssetIdChanged() {
            base.OnAssetIdChanged();

            if (this.AssetId != Guid.Empty && this.SpriteSheet is SpriteSheet spriteSheet) {
                if (this.Type.IsAssignableFrom(typeof(SpriteAnimation))) {
                    if (spriteSheet.TryGetAsset(this.AssetId, out SpriteAnimation? animation)) {
                        this.Asset = animation as TAsset;
                    }
                }
                else if (this.Type.IsAssignableFrom(typeof(AutoTileSet))) {
                    if (spriteSheet.TryGetAsset(this.AssetId, out AutoTileSet? autoTileSet)) {
                        this.Asset = autoTileSet as TAsset;
                    }
                }
                else if (this.Type.IsAssignableFrom(typeof(RandomTileSet))) {
                    if (spriteSheet.TryGetAsset(this.AssetId, out RandomTileSet? randomTileSet)) {
                        this.Asset = randomTileSet as TAsset;
                    }
                }
            }
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;

    /// <summary>
    /// An asset reference for an asset packaged inside of a <see cref="SpriteSheet" />.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    public class SpriteSheetAssetReference<TAsset> : AssetReference<TAsset> where TAsset : BaseSpriteSheetAsset {
        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet? SpriteSheet => this.Asset?.SpriteSheet;

        /// <inheritdoc />
        public override void Initialize(TAsset asset) {
            if (this.Asset?.SpriteSheet is SpriteSheet originalSpriteSheet) {
                originalSpriteSheet.PropertyChanged -= this.SpriteSheet_OnPropertyChanged;
            }

            base.Initialize(asset);

            if (this.Asset?.SpriteSheet is SpriteSheet newSpriteSheet) {
                newSpriteSheet.PropertyChanged += this.SpriteSheet_OnPropertyChanged;
            }
        }

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
            }
        }

        private void SpriteSheet_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender, e);
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;

    /// <summary>
    /// An asset reference for an asset packaged inside of a <see cref="SpriteSheet" />.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    public class SpriteSheetAssetReference<TAsset> : PackagedAssetReference<SpriteSheet, TAsset> where TAsset : SpriteSheetAsset {
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

        private void SpriteSheet_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(sender, e);
        }
    }
}
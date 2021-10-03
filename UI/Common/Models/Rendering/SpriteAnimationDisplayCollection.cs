namespace Macabresoft.Macabre2D.UI.Common {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A collection of <see cref="SpriteAnimation" /> for display.
    /// </summary>
    public class SpriteAnimationDisplayCollection : SpriteSheetAssetDisplayCollection<SpriteAnimation> {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimationDisplayCollection" /> class.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The file.</param>
        public SpriteAnimationDisplayCollection(SpriteSheet spriteSheet, ContentFile file) : base(spriteSheet, file) {
        }
    }
}
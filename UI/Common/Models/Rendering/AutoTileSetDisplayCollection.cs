namespace Macabresoft.Macabre2D.UI.Common.Models.Rendering {
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    /// <summary>
    /// A collection of <see cref="AutoTileSet" /> for display.
    /// </summary>
    public class AutoTileSetDisplayCollection : SpriteSheetAssetDisplayCollection<AutoTileSet> {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetDisplayCollection" /> class.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The file.</param>
        public AutoTileSetDisplayCollection(SpriteSheet spriteSheet, ContentFile file) : base(spriteSheet, file) {
        }
    }
}
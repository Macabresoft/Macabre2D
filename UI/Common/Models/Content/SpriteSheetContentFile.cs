namespace Macabresoft.Macabre2D.UI.Common {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A <see cref="ContentFile"/> for <see cref="SpriteSheet"/>.
    /// </summary>
    public class SpriteSheetContentFile : ContentFile<SpriteSheet> {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="metadata">The metadata.</param>
        public SpriteSheetContentFile(IContentDirectory parent, ContentMetadata metadata) : base(parent, metadata) {
        }
    }
}
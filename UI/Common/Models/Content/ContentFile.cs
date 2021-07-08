namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public class ContentFile : ContentNode {

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="parent"></param>
        /// <param name="metadata">The metadata.</param>
        public ContentFile(IContentDirectory parent, ContentMetadata metadata) : base(metadata?.GetFileName() ?? string.Empty, parent) {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        public IAsset Asset => this.Metadata?.Asset;

        /// <summary>
        /// The metadata.
        /// </summary>
        public ContentMetadata Metadata { get; }

        /// <inheritdoc />
        protected override void OnPathChanged(string originalPath) {
            this.Metadata.SetContentPath(this.GetContentPath());
            base.OnPathChanged(originalPath);
        }
    }
}
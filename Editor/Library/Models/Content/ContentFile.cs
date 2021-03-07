namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public class ContentFile : ContentNode {
        /// <summary>
        /// The file extension placed on metadata files before being compiled. Replaced with .xnb during content build.
        /// </summary>
        public const string FileExtension = ".m2d";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="metadata">The metadata.</param>
        public ContentFile(IContentDirectory parent, ContentMetadata metadata) : base(metadata?.GetFileName() ?? string.Empty, parent) {
            this.Metadata = metadata;
        }

        /// <inheritdoc />
        protected override void OnPathChanged(string originalPath) {
            base.OnPathChanged(originalPath);
            this.Metadata?.SetContentPath(this.GetContentPath());
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        public IAsset Asset => this.Metadata?.Asset;

        /// <summary>
        /// The metadata.
        /// </summary>
        public ContentMetadata Metadata { get; }
    }
}
namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using Macabresoft.Macabre2D.Framework;
    using System;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public class ContentFile : ContentNode {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="metadata">The metadata.</param>
        public ContentFile(IContentDirectory parent, ContentMetadata metadata) : base(metadata?.GetFileName() ?? string.Empty, parent) {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        public IAsset Asset => this.Metadata.Asset;

        /// <inheritdoc />
        public override Guid Id => this.Metadata.ContentId;

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
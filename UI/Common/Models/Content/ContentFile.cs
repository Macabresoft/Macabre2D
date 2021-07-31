namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using Macabresoft.Macabre2D.Framework;

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
        [DataMember]
        [Category(nameof(Asset))]
        public IAsset Asset => this.Metadata.Asset;

        /// <inheritdoc />
        public override Guid Id => this.Metadata.ContentId;

        /// <summary>
        /// The metadata.
        /// </summary>
        public ContentMetadata Metadata { get; }

        /// <inheritdoc />
        protected override string GetNameWithoutExtension() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        /// <inheritdoc />
        protected override string GetFileExtension() {
            return this.Metadata.ContentFileExtension;
        }

        /// <inheritdoc />
        protected override void OnPathChanged(string originalPath) {
            this.Metadata.SetContentPath(this.GetContentPath());
            base.OnPathChanged(originalPath);
        }
    }
}
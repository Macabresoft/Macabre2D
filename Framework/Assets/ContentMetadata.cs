namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Serializable metadata for content loaded by MonoGame. Contains the assets built on top of content.
    /// </summary>
    [DataContract]
    public class ContentMetadata {
        /// <summary>
        /// The directory name for archived metadata.
        /// </summary>
        public const string ArchiveDirectoryName = ".archive";

        /// <summary>
        /// The file extension for metadata.
        /// </summary>
        public const string FileExtension = ".meta";

        /// <summary>
        /// The directory name for the metadata.
        /// </summary>
        public const string MetadataDirectoryName = ".metadata";

        /// <summary>
        /// The search pattern for metadata files.
        /// </summary>
        public const string MetadataSearchPattern = "*" + FileExtension;

        [DataMember]
        private List<string> _splitContentPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <param name="asset">The asses which manages this content.</param>
        /// <param name="splitContentPath">The split path to the content file without its extension.</param>
        /// <param name="contentFileExtension"></param>
        public ContentMetadata(IAsset? asset, IEnumerable<string> splitContentPath, string contentFileExtension) {
            this.Asset = asset;
            this.ContentFileExtension = contentFileExtension;
            this._splitContentPath = splitContentPath?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        [DataMember]
        public IAsset? Asset { get; }

        /// <summary>
        /// Gets the content file's extension, including the period.
        /// </summary>
        [DataMember]
        public string ContentFileExtension { get; }

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        public Guid ContentId => this.Asset?.ContentId ?? Guid.Empty;

        /// <summary>
        /// Gets the content's path.
        /// </summary>
        public IReadOnlyCollection<string> SplitContentPath => this._splitContentPath;

        /// <summary>
        /// Gets the archive path from a given identifier.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The archive path.</returns>
        public static string GetArchivePath(Guid contentId) {
            return Path.Combine(ArchiveDirectoryName, $"{contentId.ToString()}{FileExtension}");
        }

        /// <summary>
        /// Gets the content path.
        /// </summary>
        /// <returns>The split content path.</returns>
        public string GetContentPath() {
            return Path.Combine(this._splitContentPath.ToArray());
        }

        /// <summary>
        /// Gets the name of the content file associated with this metadata and includes the file's extension.
        /// </summary>
        /// <returns>The name of the content file.</returns>
        public string GetFileName() {
            return !string.IsNullOrWhiteSpace(this.ContentFileExtension) ? $"{this.GetFileNameWithoutExtension()}{this.ContentFileExtension}" : this.GetFileNameWithoutExtension();
        }

        /// <summary>
        /// Gets the name of the content file associated with this metadata, but does not include the file's extension.
        /// </summary>
        /// <returns>The name of the content file.</returns>
        public string GetFileNameWithoutExtension() {
            return this._splitContentPath.Any() ? this._splitContentPath.Last() : string.Empty;
        }

        /// <summary>
        /// Gets the metadata path from a given identifier.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The metadata path.</returns>
        public static string GetMetadataPath(Guid contentId) {
            return Path.Combine(MetadataDirectoryName, $"{contentId.ToString()}{FileExtension}");
        }

        /// <summary>
        /// Gets the content build commands used by MGCB to compile this metadata and its associated asset and content.
        /// </summary>
        /// <returns>The content build commands.</returns>
        public string GetContentBuildCommands() {
            var contentStringBuilder = new StringBuilder();

            if (this.Asset != null) {
                var contentPath = this.GetContentPath();
                var metadataPath = GetMetadataPath(this.ContentId);
                contentStringBuilder.AppendLine($"# name --------- {this.GetFileName()}");
                contentStringBuilder.AppendLine($"# content path - {contentPath}");
                contentStringBuilder.AppendLine($"# content id --- {this.ContentId}");
                contentStringBuilder.AppendLine($"#begin {metadataPath}");
                contentStringBuilder.AppendLine($@"/importer:{nameof(MetadataImporter)}");
                contentStringBuilder.AppendLine($@"/processor:{nameof(MetadataProcessor)}");
                contentStringBuilder.AppendLine($@"/build:{metadataPath}");
                contentStringBuilder.AppendLine($"#end {metadataPath}");
                contentStringBuilder.AppendLine();
                contentStringBuilder.AppendLine(this.Asset.GetContentBuildCommands(contentPath, this.ContentFileExtension));
                contentStringBuilder.AppendLine($"# --------------");
            }

            return contentStringBuilder.ToString();
        }

        /// <summary>
        /// Sets the content path to a new value.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        public void SetContentPath(string contentPath) {
            this._splitContentPath = contentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
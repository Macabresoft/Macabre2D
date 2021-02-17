namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// Serializable metadata for content loaded by MonoGame. Contains the assets built on top of content.
    /// </summary>
    [DataContract]
    public class ContentMetadata {
        /// <summary>
        /// The file extension for metadata.
        /// </summary>
        public const string FileExtension = ".meta";

        /// <summary>
        /// The directory name for the metadata.
        /// </summary>
        public const string MetadataDirectoryName = ".metadata";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <remarks>This constructor should only be used when metadata is being deserialized.</remarks>
        internal ContentMetadata() : this(null, string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <param name="asset">The asses which manages this content.</param>
        /// <param name="contentPath">The path to the content file without its extension.</param>
        public ContentMetadata(IAsset? asset, string contentPath) {
            this.Asset = asset;
            this.ContentPath = contentPath;
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        [DataMember]
        public IAsset? Asset { get; }

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        public Guid ContentId => this.Asset?.ContentId ?? Guid.Empty;

        /// <summary>
        /// Gets the content's path.
        /// </summary>
        [DataMember]
        public string ContentPath { get; }

        /// <summary>
        /// Gets the metadata path from a given identifier.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <returns>The metadata path.</returns>
        public static string GetMetadataPath(Guid contentId) {
            return Path.Combine(MetadataDirectoryName, $"{contentId.ToString()}{FileExtension}");
        }
    }
}
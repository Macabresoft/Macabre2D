namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
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

        [DataMember]
        private readonly HashSet<IAsset> _assets = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <remarks>This constructor should only be used when metadata is being deserialized.</remarks>
        internal ContentMetadata() : this(Guid.Empty, null) {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentMetadata" /> class.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="asset">The asses which manages this content.</param>
        public ContentMetadata(Guid contentId, IAsset? asset) {
            this.ContentId = contentId;
            this.Asset = asset;
        }
        
        /// <summary>
        /// Gets the asset.
        /// </summary>
        [DataMember]
        public IAsset? Asset { get; private set; }

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        [DataMember]
        public Guid ContentId { get; }
    }
}
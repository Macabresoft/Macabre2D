namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public abstract class ContentFile : ContentNode {
        /// <summary>
        /// The file extension placed on metadata files before being compiled. Replaced with .xnb during content build.
        /// </summary>
        public const string FileExtension = ".m2d";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="metadata">The metadata.</param>
        protected ContentFile(string name, ContentMetadata metadata) : base(name) {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        public IAsset Asset => this.Metadata?.Asset;
        
        /// <summary>
        /// The metadata.
        /// </summary>
        protected ContentMetadata Metadata { get; }
    }

    /// <summary>
    /// A content file for the project which is aware of the type of content.
    /// </summary>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public abstract class ContentFile<TContent> : ContentFile where TContent : class {
        /// <inheritdoc />
        protected ContentFile(string name, ContentMetadata metadata) : base(name, metadata) {
        }
    }
}
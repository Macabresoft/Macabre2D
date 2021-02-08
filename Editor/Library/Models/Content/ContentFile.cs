namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public abstract class ContentFile : ContentNode {
        /// <inheritdoc />
        protected ContentFile(string name, ContentMetadata metadata) : base(name) {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the assets.
        /// </summary>
        public abstract IReadOnlyCollection<IAsset> Assets { get; }
        
        /// <summary>
        /// The metadata.
        /// </summary>
        protected ContentMetadata Metadata { get; }

        /// <summary>
        /// Adds an asset based on type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The value;</returns>
        public abstract bool AddAsset(Type type);

        /// <summary>
        /// Gets all available types of assets that can be created for this piece of content.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Type> GetCreatableAssetTypes();

        /// <summary>
        /// Removes the asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>A value indicating whether or not the asset was removed.</returns>
        public abstract bool RemoveAsset(IAsset asset);
    }

    /// <summary>
    /// A content file for the project which is aware of the type of content.
    /// </summary>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public abstract class ContentFile<TContent> : ContentFile where TContent : class {
        /// <inheritdoc />
        protected ContentFile(string name, ContentMetadata metadata) : base(name, metadata) {
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<IAsset> Assets => this.Metadata.Assets;

        /// <inheritdoc />
        public override bool RemoveAsset(IAsset asset) {
            var result = false;

            if (asset is IContentAsset<TContent> contentAsset) {
                result = this.Metadata.RemoveAsset(contentAsset);
            }

            return result;
        }

        /// <summary>
        /// Adds the asset to this content.
        /// </summary>
        /// <param name="asset">The asset.</param>
        protected void AddAsset(IContentAsset<TContent> asset) {
            this.Metadata.AddAsset(asset);
        }
    }
}
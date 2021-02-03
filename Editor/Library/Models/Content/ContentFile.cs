namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System;
    using System.Collections.Generic;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    public abstract class ContentFile : ContentNode {
        /// <inheritdoc />
        public ContentFile(string name) : base(name) {
        }

        /// <summary>
        /// Gets the assets.
        /// </summary>
        public abstract IReadOnlyCollection<IAsset> Assets { get; }

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
        private readonly ObservableCollectionExtended<IContentAsset<TContent>> _assets = new();

        /// <inheritdoc />
        protected ContentFile(string name) : base(name) {
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<IContentAsset<TContent>> Assets => this._assets;

        /// <inheritdoc />
        public override bool RemoveAsset(IAsset asset) {
            var result = false;

            if (asset is IContentAsset<TContent> contentAsset) {
                result = this._assets.Remove(contentAsset);
            }

            return result;
        }

        /// <summary>
        /// Adds the asset to this content.
        /// </summary>
        /// <param name="asset">The asset.</param>
        protected void AddAsset(IContentAsset<TContent> asset) {
            this._assets.Add(asset);
        }
    }
}
namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An asset package. Contains a single piece of content that many assets access.
    /// </summary>
    public interface IAssetPackage {
        /// <summary>
        /// Gets the content identifier for which this package is responsible.
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Initializes this asset package.
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// A definition of <see cref="IAssetPackage"/> which has the asset defined.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    public interface IAssetPackage<TAsset> : IAssetPackage where TAsset : IAsset {
        /// <summary>
        /// Gets the assets owned by this package.
        /// </summary>
        IReadOnlyCollection<TAsset> Assets { get; }

        /// <summary>
        /// Adds an asset.
        /// </summary>
        /// <returns>The added asset.</returns>
        TAsset AddAsset();

        /// <summary>
        /// Removes an asset.
        /// </summary>
        /// <param name="asset">The asset to remove.</param>
        /// <returns>A value indicating whether or not the asset was removed.</returns>
        bool RemoveAsset(TAsset asset);

        /// <summary>
        /// Removes an asset based on its identifier.
        /// </summary>
        /// <param name="assetId">The identifier of the asset to remove.</param>
        /// <returns>The asset.</returns>
        bool RemoveAsset(Guid assetId);

        /// <summary>
        /// Gets an asset by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="asset">The found asset if it exists.</param>
        /// <returns>A value indicating whether or not the asset was found.</returns>
        bool TryGetAsset(Guid id, out TAsset? asset);
    }

    /// <summary>
    /// A definition of <see cref="IAssetPackage"/> which has the content defined.
    /// </summary>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public interface IAssetPackageWithContent<TContent> : IAssetPackage where TContent : class {
        /// <summary>
        /// Gets the content for which this package is responsible.
        /// </summary>
        TContent? Content { get; }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content.</param>
        void LoadContent(TContent content);
    }

    /// <summary>
    /// A definition of a <see cref="IAssetPackage"/> which has both the asset and content defined.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public interface IAssetPackage<TAsset, TContent> : IAssetPackage<TAsset>, IAssetPackageWithContent<TContent> where TAsset : IAsset where TContent : class {
    }
}
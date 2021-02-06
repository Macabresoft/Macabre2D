namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// A package that contains other assets.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    public interface IAssetPackage<TAsset> where TAsset : IAsset {
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
    /// A definition of a <see cref="IAssetPackage{TAsset}" /> which has both the asset and content defined.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public interface IAssetPackage<TAsset, TContent> : IAssetPackage<TAsset>, IContentAsset<TContent> where TAsset : IAsset where TContent : class {
    }
}
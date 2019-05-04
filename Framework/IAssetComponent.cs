namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Interface for a component that has assets.
    /// </summary>
    public interface IAssetComponent {

        /// <summary>
        /// Determines whether the this component has an asset with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// <c>true</c> if this component has an asset with the specified identifier; otherwise, <c>false</c>.
        /// </returns>
        bool HasAsset(Guid id);

        /// <summary>
        /// Removes the asset with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A value indicating whether or not the asset was removed.</returns>
        bool RemoveAsset(Guid id);
    }

    /// <summary>
    /// Interface for a component that has assets of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of asset.</typeparam>
    public interface IAssetComponent<T> : IAssetComponent {

        /// <summary>
        /// Replaces the asset with a new instance.
        /// </summary>
        /// <param name="currentId">The current identifier before.</param>
        /// <param name="newAsset">The new asset.</param>
        void ReplaceAsset(Guid currentId, T newAsset);

        /// <summary>
        /// Tries to get an asset on this component with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="asset">The asset.</param>
        /// <returns>A value indicating whether or not the asset was found.</returns>
        bool TryGetAsset(Guid id, out T asset);
    }
}
namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;

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
    public interface IAssetComponent<T> : IAssetComponent where T : IIdentifiable {

        /// <summary>
        /// Gets the owned asset identifiers on this object.
        /// </summary>
        /// <returns>The owned asset identifiers.</returns>
        IEnumerable<Guid> GetOwnedAssetIds();

        /// <summary>
        /// Refreshes the asset with a new instance.
        /// </summary>
        /// <remarks>
        /// This is mostly used by the editor to consolidate assets on load. Then editing on sprite
        /// instance edits them all, because they are the same.
        /// </remarks>
        /// <param name="newAssetInstance">The new instance of the asset.</param>
        void RefreshAsset(T newInstance);

        /// <summary>
        /// Tries to get an asset on this component with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="asset">The asset.</param>
        /// <returns>A value indicating whether or not the asset was found.</returns>
        bool TryGetAsset(Guid id, out T asset);
    }
}
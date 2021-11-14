namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// Interface for an asset that packages other objects.
    /// </summary>
    public interface IAssetPackage {
        /// <summary>
        /// Tries to get the packaged object based off of its identifier.
        /// </summary>
        /// <param name="id">The identifier of the packaged object.</param>
        /// <param name="packaged">The packaged object, if found.</param>
        /// <typeparam name="TPackaged">The type of the packaged object.</typeparam>
        /// <returns>A value indicating whether or not the package was found.</returns>
        bool TryGetPackaged<TPackaged>(Guid id, out TPackaged? packaged) where TPackaged : class, IIdentifiable;

        /// <summary>
        /// Checks whether or not this package has an asset.
        /// </summary>
        /// <param name="packagedAssetId">The identifier of the packaged asset.</param>
        /// <returns>A value indicating whether or not this package has an asset.</returns>
        bool HasPackagedAsset(Guid packagedAssetId);
    }

    /// <summary>
    /// An asset that packages other objects.
    /// </summary>
    [DataContract]
    public abstract class AssetPackage<TContent> : Asset<TContent>, IAssetPackage {
        [DataMember]
        private readonly ObservableCollectionExtended<IIdentifiable> _packagedObjects = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetPackage{TContent}" /> class.
        /// </summary>
        protected AssetPackage() : base() {
        }

        /// <inheritdoc />
        public bool TryGetPackaged<TPackaged>(Guid id, out TPackaged? packaged) where TPackaged : class, IIdentifiable {
            packaged = this.GetPackages().FirstOrDefault(x => x.Id == id) as TPackaged;
            return packaged != null;
        }

        /// <inheritdoc />
        public bool HasPackagedAsset(Guid packagedAssetId) {
            return this.GetPackages().Any(x => x.Id == packagedAssetId);
        }

        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <returns>The packages.</returns>
        protected abstract IEnumerable<IIdentifiable> GetPackages();
    }
}
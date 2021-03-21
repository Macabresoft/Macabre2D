namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// Interface for an asset that packages other objects.
    /// </summary>
    public interface IAssetPackage {
        /// <summary>
        /// Adds the packaged object.
        /// </summary>
        /// <param name="packaged">The packaged object to add.</param>
        /// <typeparam name="TPackaged">The type of the packaged object.</typeparam>
        /// <returns>A value indicating whether or not the package was added.</returns>
        bool AddPackage<TPackaged>(TPackaged packaged) where TPackaged : class, IIdentifiable;

        /// <summary>
        /// Removes the packaged object.
        /// </summary>
        /// <param name="packaged">The packaged object to remove.</param>
        /// <typeparam name="TPackaged">The type of the packaged object.</typeparam>
        /// <returns>A value indicating whether or not the package was removed.</returns>
        bool RemovePackage<TPackaged>(TPackaged packaged) where TPackaged : class, IIdentifiable;

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

        /// <inheritdoc />
        public bool AddPackage<TPackaged>(TPackaged packaged) where TPackaged : class, IIdentifiable {
            var result = false;
            if (this._packagedObjects.All(x => x.Id != packaged.Id)) {
                this._packagedObjects.Add(packaged);
                result = true;
            }

            return result;
        }

        /// <inheritdoc />
        public bool RemovePackage<TPackaged>(TPackaged packaged) where TPackaged : class, IIdentifiable {
            return this._packagedObjects.Remove(packaged);
        }

        /// <inheritdoc />
        public bool TryGetPackaged<TPackaged>(Guid id, out TPackaged? packaged) where TPackaged : class, IIdentifiable {
            packaged = this._packagedObjects.FirstOrDefault(x => x.Id == id) as TPackaged;
            return packaged != null;
        }

        /// <inheritdoc />
        public bool HasPackagedAsset(Guid packagedAssetId) {
            return this._packagedObjects.Any(x => x.Id == packagedAssetId);
        }
    }
}
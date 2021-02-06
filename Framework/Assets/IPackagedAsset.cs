namespace Macabresoft.Macabre2D.Framework {
    using System;

    /// <summary>
    /// An asset which is packaged.
    /// </summary>
    public interface IPackagedAsset<TPackage> {
        /// <summary>
        /// Gets the package.
        /// </summary>
        TPackage? Package { get; }

        /// <summary>
        /// Gets the package identifier.
        /// </summary>
        Guid PackageId { get; }

        /// <summary>
        /// Initializes this asset with its owning package.
        /// </summary>
        /// <param name="owningPackage">The package which owns this asset.</param>
        void Initialize(TPackage owningPackage);
    }
}
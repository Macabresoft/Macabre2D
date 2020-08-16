namespace Macabresoft.MonoGame.Core {

    using System;

    /// <summary>
    /// Interface for an object that is an asset.
    /// </summary>
    public interface IAsset {

        /// <summary>
        /// Gets or sets the asset identifier.
        /// </summary>
        /// <value>The asset identifier.</value>
        Guid AssetId { get; set; }
    }
}
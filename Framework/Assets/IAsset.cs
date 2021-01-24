namespace Macabresoft.Macabre2D.Framework {

    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface for an object that is an asset.
    /// </summary>
    public interface IAsset : INotifyPropertyChanged {

        /// <summary>
        /// Gets the asset identifier.
        /// </summary>
        /// <value>The asset identifier.</value>
        Guid AssetId { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}
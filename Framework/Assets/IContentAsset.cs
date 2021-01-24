namespace Macabresoft.Macabre2D.Framework {
    using System;

    /// <summary>
    /// Interface for an asset that contains content.
    /// </summary>
    public interface IContentAsset<TContent> : IAsset {
        /// <summary>
        /// Gets the identifier for the content that this asset contains.
        /// </summary>
        Guid ContentId { get; }
        
        /// <summary>
        /// Gets the content.
        /// </summary>
        TContent? Content { get; }

        /// <summary>
        /// Initializes the asset with its content.
        /// </summary>
        /// <param name="content">The content.</param>
        void Initialize(TContent content);
    }
}
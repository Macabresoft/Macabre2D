namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an asset that contains content.
    /// </summary>
    public interface IContentAsset<TContent> : IAsset {
        /// <summary>
        /// Gets the content.
        /// </summary>
        TContent? Content { get; }

        /// <summary>
        /// Gets the identifier for the content that this asset contains.
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Loads content for this asset.
        /// </summary>
        /// <param name="content">The content.</param>
        void LoadContent(TContent content);
    }

    /// <summary>
    /// A base class for assets which implement <see cref="IContentAsset{TContent}" />.
    /// </summary>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public class ContentAsset<TContent> : Asset, IContentAsset<TContent> where TContent : class {
        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public TContent? Content { get; private set; }

        /// <inheritdoc />
        public virtual void LoadContent(TContent content) {
            this.Content = content;
        }
    }
}
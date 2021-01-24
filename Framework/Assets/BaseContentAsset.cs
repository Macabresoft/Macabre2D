namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base class for assets which implement <see cref="IContentAsset{TContent}"/>.
    /// </summary>
    /// <typeparam name="TContent">The type of content.</typeparam>
    public class BaseContentAsset<TContent> : BaseAsset, IContentAsset<TContent> where TContent : class {
        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; set; }
        
        /// <inheritdoc />
        public TContent? Content { get; private set; }
        
        /// <inheritdoc />
        public virtual void Initialize(TContent content) {
            this.Content = content;
        }
    }
}
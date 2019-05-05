using System;

namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for identifiable content.
    /// </summary>
    public interface IIdentifiableContent {

        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        Guid ContentId { get; }
    }
}
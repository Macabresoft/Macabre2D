using System;

namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for a component that has identifiable content.
    /// </summary>
    public interface IIdentifiableContentComponent {

        /// <summary>
        /// Determines whether the this component has content with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// <c>true</c> if this component has content with the specified identifier; otherwise, <c>false</c>.
        /// </returns>
        bool HasContent(Guid id);

        /// <summary>
        /// Removes the content with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void RemoveContent(Guid id);
    }
}
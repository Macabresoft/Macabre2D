namespace Macabre2D.Framework {

    /// <summary>
    /// Interface for a component that has one or more content paths that can be updated.
    /// </summary>
    public interface IContentComponent {

        /// <summary>
        /// Updates the content path of a component.
        /// </summary>
        /// <param name="oldPath">The old path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns>A value indicating whether or not a path was updated.</returns>
        bool UpdateContentPath(string oldPath, string newPath);
    }
}
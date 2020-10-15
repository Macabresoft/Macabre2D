namespace Macabresoft.Macabre2D.Framework {

    /// <summary>
    /// Interface for physics objects that can be contained within a bounding area.
    /// </summary>
    public interface IBoundable {

        /// <summary>
        /// Gets the bounding area.
        /// </summary>
        /// <value>The bounding area.</value>
        BoundingArea BoundingArea { get; }
    }
}
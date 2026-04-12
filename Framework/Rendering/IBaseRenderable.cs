namespace Macabre2D.Framework;

using Macabre2D.Project.Common;

/// <summary>
/// Includes all the basic information that any rendered entity would contain.
/// </summary>
public interface IBaseRenderable : IBoundableEntity {
    /// <summary>
    /// Gets or sets the render order.
    /// </summary>
    int RenderOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this should be rendered when out of bounds.
    /// </summary>
    bool RenderOutOfBounds { get; set; }

    /// <summary>
    /// Gets the render priority.
    /// </summary>
    RenderPriority RenderPriority { get; set; }
}
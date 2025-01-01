namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// An interface for dockable entities to be arranged by a <see cref="DockingContainer" />.
/// </summary>
public interface IDockable : IBoundableEntity, ITransformable {
    /// <summary>
    /// Gets or sets the dock location.
    /// </summary>
    DockLocation Location { get; set; }
}
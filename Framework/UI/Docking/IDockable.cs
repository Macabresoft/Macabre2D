namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// An interface for dockable entities to be arranged by a <see cref="DockingContainer" />.
/// </summary>
public interface IDockable : IBoundable, ITransformable {
    /// <summary>
    /// Gets or sets the dock location.
    /// </summary>
    DockLocation Location { get; set; }

    /// <summary>
    /// Gets or sets the margin.
    /// </summary>
    DockingMargin Margin { get; set; }
}
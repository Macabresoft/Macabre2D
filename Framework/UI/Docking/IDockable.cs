namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;

/// <summary>
/// An interface for dockable entities to be arranged by a <see cref="DockingContainer"/>.
/// </summary>
public interface IDockable : IBoundable, ITransformable {

    /// <summary>
    /// Fires off when a rearrange is requested. This can happen when <see cref="BoundingArea"/> or <see cref="Location"/> change.
    /// </summary>
    event EventHandler RearrangeRequested;
    
    /// <summary>
    /// Gets or sets the dock location.
    /// </summary>
    DockLocation Location { get; set; }
}
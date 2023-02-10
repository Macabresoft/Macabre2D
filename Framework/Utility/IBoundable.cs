namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// Interface for objects that can be contained within a bounding area. Generally used for physics and rendering.
/// </summary>
public interface IBoundable {
    /// <summary>
    /// Gets the bounding area.
    /// </summary>
    /// <value>The bounding area.</value>
    BoundingArea BoundingArea { get; }

    /// <summary>
    /// Called when the bounding area changes.
    /// </summary>
    event EventHandler BoundingAreaChanged;
}
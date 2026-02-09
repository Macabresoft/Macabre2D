namespace Macabre2D.Framework;

using System;

/// <summary>
/// Interface for objects that can be contained within a bounding area. Generally used for physics and rendering.
/// </summary>
public interface IBoundable {

    /// <summary>
    /// Called when the bounding area changes.
    /// </summary>
    event EventHandler BoundingAreaChanged;

    /// <summary>
    /// Gets the bounding area.
    /// </summary>
    /// <value>The bounding area.</value>
    BoundingArea BoundingArea { get; }
}

/// <summary>
/// A <see cref="IBoundable" /> that is also an <see cref="IEntity" />.
/// </summary>
public interface IBoundableEntity : IBoundable, IEntity {
}

/// <summary>
/// Helper class for <see cref="IBoundableEntity" />.
/// </summary>
public static class Boundable {
    /// <summary>
    /// Gets a value indicating whether the specified <see cref="IBoundableEntity" /> has an empty <see cref="BoundingArea" />.
    /// </summary>
    /// <param name="boundable">The boundable.</param>
    /// <returns>A value indicating whether the boundable is empty.</returns>
    public static bool IsEmpty(this IBoundableEntity boundable) => boundable.BoundingArea.IsEmpty;
}
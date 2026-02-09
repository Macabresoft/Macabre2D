namespace Macabre2D.Framework;

using System;

/// <summary>
/// Interface for things that can be identified with a <see cref="Guid" />.
/// </summary>
public interface IIdentifiable {
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    Guid Id { get; set; }
}
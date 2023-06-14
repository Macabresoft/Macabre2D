namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A wrapper for entities being filtered for a custom tree view.
/// </summary>
public class FilteredEntityWrapper {
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredEntityWrapper" /> class.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="entityType">The entity type to find.</param>
    public FilteredEntityWrapper(IEntity entity, Type entityType) {
        if (entityType == null) {
            throw new ArgumentNullException(nameof(entityType));
        }

        this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        this.EntityType = this.Entity.GetType();
        this.IsSelectable = entityType.IsInstanceOfType(this.Entity);

        var children = new List<FilteredEntityWrapper>(
            this.Entity.Children
                .Where(x => this.IsSelectable || this.Entity.GetDescendants(entityType).Any())
                .Select(x => new FilteredEntityWrapper(x, entityType)));

        this.Children = children;
    }

    /// <summary>
    /// Gets the children of this entity if any exist.
    /// </summary>
    public IEnumerable<FilteredEntityWrapper> Children { get; }

    /// <summary>
    /// Gets the entity this is wrapping.
    /// </summary>
    public IEntity Entity { get; }

    /// <summary>
    /// Gets the entity type.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets a value specifying whether or not this is selectable.
    /// </summary>
    public bool IsSelectable { get; }
}
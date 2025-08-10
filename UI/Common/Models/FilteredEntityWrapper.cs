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
    /// Initializes a new instance of the <see cref="FilteredEntityWrapper" /> class for searching on type.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="entityType">The entity type to find.</param>
    public FilteredEntityWrapper(IEntity entity, Type entityType) : this(entity) {
        ArgumentNullException.ThrowIfNull(entityType);

        this.IsSelectable = entityType.IsInstanceOfType(this.Entity);

        var children = new List<FilteredEntityWrapper>(
                this.Entity.Children
                .Where(x => this.IsSelectable || this.Entity.GetDescendants(entityType).Any())
                .Select(x => new FilteredEntityWrapper(x, entityType))
                .Where(x => x.IsSelectable || x.Entity.GetDescendants(entityType).Any())).ToList();

        this.Children = children;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredEntityWrapper" /> class for searching on content identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="contentId">The content identifier.</param>
    public FilteredEntityWrapper(IEntity entity, Guid contentId) : this(entity) {
        this.ContentId = contentId;
        this.IsSelectable = this.Entity.ReferencesContent(this.ContentId);

        var children = new List<FilteredEntityWrapper>(
            this.Entity.Children
                .Where(x => x.ReferencesContent(contentId) || x.GetDescendantsWithContent(this.ContentId).Any())
                .Select(x => new FilteredEntityWrapper(x, contentId)));

        this.Children = children;
    }

    private FilteredEntityWrapper(IEntity entity) {
        this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        this.EntityType = this.Entity.GetType();
    }

    /// <summary>
    /// Gets the children of this entity if any exist.
    /// </summary>
    public IEnumerable<FilteredEntityWrapper> Children { get; }

    /// <summary>
    /// Gets the content type.
    /// </summary>
    public Guid ContentId { get; } = Guid.Empty;

    /// <summary>
    /// Gets the entity this is wrapping.
    /// </summary>
    public IEntity Entity { get; }

    /// <summary>
    /// Gets the entity type.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets a value specifying whether this is selectable.
    /// </summary>
    public bool IsSelectable { get; }
}
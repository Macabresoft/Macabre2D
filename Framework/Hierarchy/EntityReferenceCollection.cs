namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a collection of entity references.
/// </summary>
public interface IEntityReferenceCollection : IEntityReference {
    /// <summary>
    /// Gets the entity identifiers referenced in this collection.
    /// </summary>
    IReadOnlyCollection<Guid> EntityIds { get; }

    /// <summary>
    /// Gets a value indicating whether this has any entities assigned to it.
    /// </summary>
    /// <remarks>
    /// This looks at <see cref="EntityIds" />, so it can return true even if <see cref="UntypedEntities" /> appears empty.
    /// </remarks>
    bool HasEntities { get; }

    /// <summary>
    /// Gets the entities as <see cref="IEntity" /> and not their specific type.
    /// </summary>
    IReadOnlyCollection<IEntity> UntypedEntities { get; }

    /// <summary>
    /// Adds the entity to this collection if it is not already present.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    void AddEntity(Guid id);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes the entity from this collection.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    void RemoveEntity(Guid id);
}

/// <summary>
/// A collection of entity references.
/// </summary>
[DataContract]
public abstract class EntityReferenceCollection : PropertyChangedNotifier, IEntityReferenceCollection {
    [DataMember]
    private readonly HashSet<Guid> _entityIds = new();

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> EntityIds => this._entityIds;

    /// <inheritdoc />
    public bool HasEntities => this._entityIds.Any();

    /// <inheritdoc />
    public abstract Type Type { get; }

    /// <inheritdoc />
    public abstract IReadOnlyCollection<IEntity> UntypedEntities { get; }

    /// <summary>
    /// Gets a value indicating whether this is intialized.
    /// </summary>
    protected bool IsInitialized => !Framework.Scene.IsNullOrEmpty(this.Scene);

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = Framework.Scene.Empty;

    /// <inheritdoc />
    public abstract void AddEntity(Guid id);

    /// <inheritdoc />
    public virtual void Clear() {
        this._entityIds.Clear();
    }

    /// <inheritdoc />
    public virtual void Deinitialize() {
        this.Scene = Framework.Scene.Empty;
    }

    /// <inheritdoc />
    public virtual void Initialize(IScene scene) {
        this.Scene = scene;
    }

    /// <inheritdoc />
    public abstract void RemoveEntity(Guid id);

    /// <summary>
    /// Adds the entity identifier to the collection.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>A value indicating whether the entity identifier could be added to the collection.</returns>
    protected bool AddEntityId(Guid id) => this._entityIds.Add(id);

    /// <summary>
    /// Removes the entity identifier to the collection.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>A value indicating whether the entity identifier existed in the collection and required removing.</returns>
    protected bool RemoveEntityId(Guid id) => this._entityIds.Remove(id);
}

/// <summary>
/// Collection of entity references.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class EntityReferenceCollection<TEntity> : EntityReferenceCollection where TEntity : IEntity {
    private readonly List<TEntity> _entities = new();

    /// <summary>
    /// Gets the entities as <see cref="TEntity" />.
    /// </summary>
    public IReadOnlyCollection<TEntity> Entities => this._entities;

    /// <inheritdoc />
    public override Type Type => typeof(TEntity);


    /// <inheritdoc />
    public override IReadOnlyCollection<IEntity> UntypedEntities => this._entities.Cast<IEntity>().ToList();

    /// <inheritdoc />
    public override void AddEntity(Guid id) {
        if (this.IsInitialized && this.Scene.FindChild(id) is TEntity entity && this.AddEntityId(id)) {
            this._entities.Add(entity);
        }
    }

    /// <summary>
    /// Adds the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void AddEntity(TEntity entity) {
        if (this.AddEntityId(entity.Id)) {
            this._entities.Add(entity);
        }
    }

    /// <inheritdoc />
    public override void Clear() {
        base.Clear();
        this._entities.Clear();
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._entities.Clear();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);
        this.ResetEntities();
    }

    /// <inheritdoc />
    public override void RemoveEntity(Guid id) {
        if (this.RemoveEntityId(id) && this._entities.FirstOrDefault(x => x.Id == id) is { } entity) {
            this._entities.Remove(entity);
        }
    }

    private void ResetEntities() {
        this._entities.Clear();

        if (this.IsInitialized) {
            foreach (var entityId in this.EntityIds.ToList()) {
                if (this.Scene.FindChild(entityId) is TEntity entity) {
                    this._entities.Add(entity);
                }
                else {
                    this.RemoveEntityId(entityId);
                }
            }
        }
    }
}
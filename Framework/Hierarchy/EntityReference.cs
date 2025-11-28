namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a reference to an entity.
/// </summary>
public interface IEntityReference : IGameObjectReference {

    /// <summary>
    /// Gets the type of the entity referenced.
    /// </summary>
    Type Type { get; }
}

/// <summary>
/// Base class for entity references.
/// </summary>
[DataContract]
public abstract class EntityReference : PropertyChangedNotifier, IEntityReference {
    private Guid _entityId;

    /// <inheritdoc />
    public abstract Type Type { get; }

    /// <summary>
    /// Gets an untyped version of the entity.
    /// </summary>
    public abstract IEntity? UntypedEntity { get; }

    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    [DataMember]
    [EntityGuid]
    public Guid EntityId {
        get => this._entityId;
        set {
            if (this._entityId != value) {
                this._entityId = value;
                this.ResetEntity();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = EmptyObject.Scene;

    /// <inheritdoc />
    public virtual void Deinitialize() {
        this.Scene = EmptyObject.Scene;
    }

    /// <inheritdoc />
    public void Initialize(IScene scene) {
        this.Scene = scene;
        this.ResetEntity();
    }

    /// <summary>
    /// Sets the entity.
    /// </summary>
    protected abstract void ResetEntity();
}

/// <summary>
/// A reference to an entity using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class EntityReference<TEntity> : EntityReference where TEntity : class, IEntity {
    private TEntity? _entity;

    /// <inheritdoc />
    public override Type Type => typeof(TEntity);

    /// <inheritdoc />
    public override IEntity? UntypedEntity => this.Entity;

    /// <summary>
    /// Gets the entity.
    /// </summary>
    public TEntity? Entity {
        get => this._entity;
        private set => this.Set(ref this._entity, value);
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._entity = null;
    }

    /// <inheritdoc />
    protected override void ResetEntity() {
        if (this.EntityId == Guid.Empty || Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Entity = null;
        }
        else {
            this.Entity = this.Scene.FindEntity<TEntity>(this.EntityId);
        }
    }
}
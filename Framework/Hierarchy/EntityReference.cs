namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A reference to an entity using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
[DataContract]
public class EntityReference<TEntity> : PropertyChangedNotifier where TEntity : class, IEntity {
    private TEntity? _entity;
    private Guid _entityId;
    private IScene _scene = Scene.Empty;

    /// <summary>
    /// Gets the entity.
    /// </summary>
    public TEntity? Entity {
        get => this._entity;
        private set => this.Set(ref this._entity, value);
    }

    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public Guid EntityId {
        get => this._entityId;
        set {
            if (this.Set(ref this._entityId, value)) {
                this.SetEntity();
            }
        }
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="scene">The scene.</param>
    public void Initialize(IScene scene) {
        this._scene = scene;
        this.SetEntity();
    }

    private void SetEntity() {
        if (this._entityId == Guid.Empty || Scene.IsNullOrEmpty(this._scene)) {
            this.Entity = null;
        }
        else {
            this.Entity = this._scene.FindEntity<TEntity>(this._entityId);
        }
    }
}
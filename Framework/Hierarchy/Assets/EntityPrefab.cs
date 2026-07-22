namespace Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// An entity prefab, which wraps an <see cref="IEntity" /> to be saved as a content file and loaded as part of an asset.
/// </summary>
[DataContract]
public class EntityPrefab {
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPrefab" /> class.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public EntityPrefab(IEntity entity) {
        this.Entity = entity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPrefab" /> class.
    /// </summary>
    public EntityPrefab() {
    }

    /// <summary>
    /// Gets the system.
    /// </summary>
    [DataMember]
    public IEntity? Entity { get; private set; }
}
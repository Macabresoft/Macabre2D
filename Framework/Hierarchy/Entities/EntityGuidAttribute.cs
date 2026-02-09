namespace Macabre2D.Framework;

using System;

/// <summary>
/// Attribute for <see cref="Guid" /> entity references.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EntityGuidAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityGuidAttribute" /> class.
    /// </summary>
    public EntityGuidAttribute() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityGuidAttribute" /> class.
    /// </summary>
    /// <param name="entityType"></param>
    public EntityGuidAttribute(Type entityType) {
        this.EntityType = entityType;
    }

    /// <summary>
    /// Gets the type of entity this <see cref="Guid" /> references.
    /// </summary>
    public Type? EntityType { get; }
}
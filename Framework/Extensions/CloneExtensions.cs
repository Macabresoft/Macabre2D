namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Diagnostics.CodeAnalysis;
using Macabresoft.Macabre2D.Common;

/// <summary>
/// Clones objects using <see cref="ISerializer" />.
/// </summary>
public static class CloneExtensions {
    /// <summary>
    /// Clones an entity.
    /// </summary>
    /// <param name="entity">The entity to clone.</param>
    /// <param name="clone">The cloned entity.</param>
    /// <returns>A value indicating whether or not the clone was successful.</returns>
    public static bool TryClone(this IEntity entity, [NotNullWhen(true)] out IEntity? clone) {
        var result = false;
        var json = Serializer.Instance.SerializeToString(entity);
        if (Serializer.Instance.DeserializeFromString(json, entity.GetType()) is IEntity tempClone) {
            result = true;
            clone = tempClone;
            clone.SetNewIds();
        }
        else {
            clone = null;
        }

        return result;
    }

    /// <summary>
    /// Clones a system.
    /// </summary>
    /// <param name="system">The system to clone.</param>
    /// <param name="clone">The cloned system.</param>
    /// <returns>A value indicating whether the clone was successful.</returns>
    public static bool TryClone(this IGameSystem system, [NotNullWhen(true)] out IGameSystem? clone) {
        var result = false;
        var json = Serializer.Instance.SerializeToString(system);
        if (Serializer.Instance.DeserializeFromString(json, system.GetType()) is IGameSystem tempClone) {
            result = true;
            clone = tempClone;
        }
        else {
            clone = null;
        }

        return result;
    }

    /// <summary>
    /// Clones an <see cref="IIdentifiable" /> object and provides it a new identifier.
    /// </summary>
    /// <param name="originalInstance">The original instance to clone.</param>
    /// <param name="clone">The cloned sprite sheet member.</param>
    /// <returns>A value indicating whether the clone was successful.</returns>
    public static bool TryClone<T>(this T originalInstance, [NotNullWhen(true)] out T? clone) where T : class, IIdentifiable {
        var result = false;
        var json = Serializer.Instance.SerializeToString(originalInstance);
        if (Serializer.Instance.DeserializeFromString(json, originalInstance.GetType()) is T tempClone) {
            result = true;
            clone = tempClone;
            clone.Id = Guid.NewGuid();
        }
        else {
            clone = null;
        }

        return result;
    }

    private static void SetNewIds(this IEntity entity) {
        entity.Id = Guid.NewGuid();
        foreach (var child in entity.Children) {
            child.SetNewIds();
        }
    }
}
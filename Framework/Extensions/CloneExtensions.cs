namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Clones objects using <see cref="ISerializer" />.
/// </summary>
public static class CloneExtensions {
    /// <summary>
    /// Clones an entity.
    /// </summary>
    /// <param name="entity">The entity to clone.</param>
    /// <param name="clone">The cloned entity.</param>
    /// <returns>A value indicating whether or not the clone was successful..</returns>
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
    /// Clones a loop.
    /// </summary>
    /// <param name="loop">The loop to clone.</param>
    /// <param name="clone">The cloned entity.</param>
    /// <returns>A value indicating whether or not the clone was successful..</returns>
    public static bool TryClone(this ILoop loop, [NotNullWhen(true)] out ILoop? clone) {
        var result = false;
        var json = Serializer.Instance.SerializeToString(loop);
        if (Serializer.Instance.DeserializeFromString(json, loop.GetType()) is ILoop tempClone) {
            result = true;
            clone = tempClone;
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
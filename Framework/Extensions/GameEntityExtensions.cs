namespace Macabresoft.Macabre2D.Framework {
    using System;

    /// <summary>
    /// Clones objects using <see cref="ISerializer" />.
    /// </summary>
    public static class GameEntityExtensions {
        /// <summary>
        /// Clones an entity.
        /// </summary>
        /// <param name="entity">The entity to clone.</param>
        /// <param name="clone">The cloned entity.</param>
        /// <returns>A cloned entity.</returns>
        public static bool TryClone(this IEntity entity, out IEntity clone) {
            var result = false;
            var json = Serializer.Instance.SerializeToString(entity);
            if (Serializer.Instance.DeserializeFromString(json, entity.GetType()) is IEntity tempClone) {
                result = true;
                clone = tempClone;
                clone.SetNewIds();
            }
            else {
                clone = Entity.Empty;
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
}
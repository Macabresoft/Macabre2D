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
        public static bool TryClone(this IGameEntity entity, out IGameEntity clone) {
            var result = false;
            var json = Serializer.Instance.SerializeToString(entity);
            if (Serializer.Instance.DeserializeFromString(json, entity.GetType()) is IGameEntity tempClone) {
                result = true;
                clone = tempClone;
                clone.SetNewIds();
            }
            else {
                clone = GameEntity.Empty;
            }

            return result;
        }

        private static void SetNewIds(this IGameEntity entity) {
            entity.Id = Guid.NewGuid();
            foreach (var child in entity.Children) {
                child.SetNewIds();
            }
        }
    }
}
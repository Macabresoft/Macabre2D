namespace Macabresoft.Macabre2D.Framework {

    using System;

    /// <summary>
    /// Clones objects using <see cref="ISerializer" />.
    /// </summary>
    public static class GameEntityExtensions {

        /// <summary>
        /// Clones a base component.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="BaseComponent" />.</typeparam>
        /// <param name="component">The component to clone.</param>
        /// <returns>A cloned component.</returns>
        public static bool TryClone(this IGameEntity entity, out IGameEntity clone) {
            var result = false;
            var json = Serializer.Instance.SerializeToString(entity);
            if (Serializer.Instance.DeserializeFromString(json, entity.GetType()) is IGameEntity tempClone) {
                result = true;
                clone = tempClone;
                clone.SetNewComponentIds();
            }
            else {
                clone = GameEntity.Empty;
            }

            return result;
        }

        private static void SetNewComponentIds(this IGameEntity entity) {
            if (entity != null) {
                foreach (var component in entity.Components) {
                    component.Id = Guid.NewGuid();
                }

                foreach (var child in entity.Children) {
                    child.SetNewComponentIds();
                }
            }
        }
    }
}
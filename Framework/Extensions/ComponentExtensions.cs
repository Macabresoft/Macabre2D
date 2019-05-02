namespace Macabre2D.Framework.Extensions {

    using Macabre2D.Framework.Serialization;
    using System;

    /// <summary>
    /// Clones objects using <see cref="ISerializer"/>.
    /// </summary>
    public static class ComponentExtensions {
        private static ISerializer _serializer = new Serializer();

        /// <summary>
        /// Clones a base component.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="BaseComponent"/>.</typeparam>
        /// <param name="component">The component to clone.</param>
        /// <returns>A cloned component.</returns>
        public static BaseComponent Clone<T>(this T component) where T : BaseComponent {
            T clone = null;
            var cloneToType = typeof(T) == typeof(BaseComponent) ? component.GetType() : typeof(T);

            if (component != null) {
                var parent = component.Parent;
                try {
                    component.Parent = null;
                    var json = ComponentExtensions._serializer.SerializeToString(component);
                    clone = ComponentExtensions._serializer.DeserializeFromString(json, cloneToType) as T;
                    var cloneParent = clone.Parent;
                    clone.SetNewComponentIds();
                    clone.Parent = parent;
                }
                finally {
                    component.Parent = parent;
                }
            }

            return clone;
        }

        private static void SetNewComponentIds(this BaseComponent component) {
            if (component != null) {
                component.Id = Guid.NewGuid();
                foreach (var child in component.Children) {
                    child.SetNewComponentIds();
                }
            }
        }
    }
}
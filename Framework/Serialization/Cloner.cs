namespace Macabre2D.Framework.Serialization {

    using System;

    public static class Cloner {
        private static ISerializer _serializer = new Serializer();

        public static BaseComponent Clone<T>(this T component) where T : BaseComponent {
            T clone = null;
            if (component != null) {
                var parent = component.Parent;
                try {
                    component.Parent = null;
                    var json = Cloner._serializer.SerializeToString(component);
                    clone = Cloner._serializer.DeserializeFromString<T>(json);
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
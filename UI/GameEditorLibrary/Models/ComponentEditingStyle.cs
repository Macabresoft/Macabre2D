namespace Macabre2D.UI.GameEditorLibrary.Models {

    using Macabre2D.Framework;
    using System;

    [Flags]
    public enum ComponentEditingStyle {
        None = 0,
        Translation = 1 << 0,
        Scale = 1 << 1,
        Rotation = 1 << 2,
        Tile = 1 << 3
    }

    public static class ComponentEditingStyleExtensions {

        public static ComponentEditingStyle GetEditingStyle(this BaseComponent component) {
            var result = ComponentEditingStyle.None;

            if (component != null) {
                var componentType = component.GetType();

                if (typeof(ITranslateable).IsAssignableFrom(componentType)) {
                    result |= ComponentEditingStyle.Translation;
                }

                if (typeof(IScaleable).IsAssignableFrom(componentType)) {
                    result |= ComponentEditingStyle.Scale;
                }

                if (typeof(IRotatable).IsAssignableFrom(componentType)) {
                    result |= ComponentEditingStyle.Rotation;
                }

                if (typeof(ITileable).IsAssignableFrom(componentType)) {
                    result |= ComponentEditingStyle.Tile;
                }
            }

            return result;
        }
    }
}
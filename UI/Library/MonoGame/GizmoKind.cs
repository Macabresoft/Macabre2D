namespace Macabresoft.Macabre2D.UI.Library.MonoGame {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Specifies the kind of gizmo.
    /// </summary>
    public enum GizmoKind {
        /// <summary>
        /// Used in selecting <see cref="IBoundable"/> in the scene editor.
        /// </summary>
        Selector = 0,
        
        /// <summary>
        /// Used in translating (moving) <see cref="ITransformable"/> in the scene editor.
        /// </summary>
        Translation = 1,
        
        /// <summary>
        /// Used in scaling (changing size) <see cref="ITransformable"/> in the scene editor.
        /// </summary>
        Scale = 2
    }
}
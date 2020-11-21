namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// Interface for a gizmo.
    /// </summary>
    public interface IGizmo {
        /// <summary>
        /// Gets the kind of gizmo this is.
        /// </summary>
        GizmoKind GizmoKind { get; }

        /// <summary>
        /// Updates the gizmo and returns a value indicating whether or not this had interactions.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="inputState">The input state.</param>
        /// <returns>A value indicating whether or not this gizmo had any interactions.</returns>
        bool Update(FrameTime frameTime, InputState inputState);
    }
}
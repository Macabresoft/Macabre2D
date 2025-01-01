namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Specifies the kind of gizmo.
/// </summary>
public enum GizmoKind {
    /// <summary>
    /// Used in selecting <see cref="IBoundableEntity" /> in the scene editor.
    /// </summary>
    Selector = 0,

    /// <summary>
    /// Used in translating (moving) <see cref="ITransformable" /> in the scene editor.
    /// </summary>
    Translation = 1,

    /// <summary>
    /// Used in placing tiles for <see cref="ITileableEntity" /> in the scene editor.
    /// </summary>
    Tile = 2
}
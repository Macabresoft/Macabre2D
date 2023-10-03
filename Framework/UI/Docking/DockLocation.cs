namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// Locations in a <see cref="DockingContainer"/> where a <see cref="IDockable"/> can be docked.
/// </summary>
public enum DockLocation {
    None,
    Center,
    Left,
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft
}
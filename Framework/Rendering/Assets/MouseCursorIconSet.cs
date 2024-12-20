namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;

/// <summary>
/// Defines the different ways a mouse cursor may appear.
/// </summary>
public enum MouseCursorAppearance {
    None,
    Standard,
    LeftClickHeld,
    Activatable
}

/// <summary>
/// A set of icons corresponding to <see cref="MouseCursorAppearance" />.
/// </summary>
public class MouseCursorIconSet : SpriteSheetIconSet<MouseCursorAppearance> {

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseCursorIconSet" /> class.
    /// </summary>
    public MouseCursorIconSet() : base() {
        this.Name = "Mouse Cursor";
    }

    /// <inheritdoc />
    public override void RefreshIcons() {
        var keys = Enum.GetValues<MouseCursorAppearance>().ToList();
        keys.Remove(MouseCursorAppearance.None);

        foreach (var key in keys) {
            this.RefreshIcon(key);
        }
    }
}
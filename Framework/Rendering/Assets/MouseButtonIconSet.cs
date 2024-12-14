namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;

/// <summary>
/// A set of icons corresponding to <see cref="MouseButton" />.
/// </summary>
public class MouseButtonIconSet : SpriteSheetIconSet<MouseButton> {
    private const string DefaultName = "Mouse Button Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseButtonIconSet" /> class.
    /// </summary>
    public MouseButtonIconSet() : base() {
        this.Name = DefaultName;
    }

    /// <inheritdoc />
    public override void RefreshIcons() {
        var keys = Enum.GetValues<MouseButton>().ToList();
        keys.Remove(MouseButton.None);

        foreach (var key in keys) {
            this.RefreshIcon(key);
        }
    }
}
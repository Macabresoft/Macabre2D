namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A set of icons corresponding to game pad <see cref="Buttons" />.
/// </summary>
public sealed class GamePadIconSet : SpriteSheetIconSet<Buttons> {
    private const string DefaultName = "Game Pad Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="GamePadIconSet" /> class.
    /// </summary>
    public GamePadIconSet() : base() {
        this.Name = DefaultName;
    }

    /// <inheritdoc />
    public override void RefreshIcons() {
        var keys = Enum.GetValues<Buttons>().ToList();
        keys.Remove(Buttons.None);

        foreach (var key in keys) {
            this.RefreshIcon(key);
        }
    }
}
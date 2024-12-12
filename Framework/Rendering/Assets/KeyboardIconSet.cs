namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A set of icons corresponding to keyboard <see cref="Keys" />.
/// </summary>
public sealed class KeyboardIconSet : SpriteSheetIconSet<Keys> {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Keyboard Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardIconSet" /> class.
    /// </summary>
    public KeyboardIconSet() : base() {
        this.Name = DefaultName;
    }
    
    /// <inheritdoc />
    public override void RefreshIcons() {
        var keys = Enum.GetValues<Keys>().ToList();
        keys.Remove(Keys.None);

        foreach (var key in keys) {
            this.RefreshIcon(key);
        }
    }
}
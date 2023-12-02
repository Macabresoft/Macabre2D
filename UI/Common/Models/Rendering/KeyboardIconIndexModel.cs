namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A model for a single icon of an <see cref="KeyboardIconSet" />.
/// </summary>
public class KeyboardIconIndexModel : BaseSpriteSheetIndexModel<KeyboardIconSet, Keys> {
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardIconIndexModel" /> class.
    /// </summary>
    /// <param name="iconSet">The icon set.</param>
    /// <param name="key">The button.</param>
    public KeyboardIconIndexModel(KeyboardIconSet iconSet, Keys key) : base(iconSet, key) {
        if (this.Member.TryGetSpriteIndex(this.Key, out var spriteIndex)) {
            this.SpriteIndex = spriteIndex;
        }
    }
}
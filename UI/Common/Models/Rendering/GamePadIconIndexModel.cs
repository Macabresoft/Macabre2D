namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A model for a single icon of an <see cref="GamePadIconSet" />.
/// </summary>
public class GamePadIconIndexModel : BaseSpriteSheetIndexModel<GamePadIconSet, Buttons> {
    /// <summary>
    /// Initializes a new instance of the <see cref="GamePadIconIndexModel" /> class.
    /// </summary>
    /// <param name="iconSet">The icon set.</param>
    /// <param name="button">The button.</param>
    public GamePadIconIndexModel(GamePadIconSet iconSet, Buttons button) : base(iconSet, button) {
        if (this.Member.TryGetSpriteIndex(this.Key, out var spriteIndex)) {
            this.SpriteIndex = spriteIndex;
        }
    }
}
namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A model for a single icon of an <see cref="GamePadIconSet" />.
/// </summary>
public class GamePadIconIndexModel : PropertyChangedNotifier {
    private readonly GamePadIconSet _iconSet;
    private byte? _spriteIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="GamePadIconIndexModel" /> class.
    /// </summary>
    /// <param name="iconSet">The icon set.</param>
    /// <param name="button">The button.</param>
    public GamePadIconIndexModel(GamePadIconSet iconSet, Buttons button) {
        this._iconSet = iconSet;
        this.Button = button;

        if (this._iconSet.TryGetSpriteIndex(this.Button, out var spriteIndex)) {
            this._spriteIndex = spriteIndex;
        }
    }

    /// <summary>
    /// Gets the button.
    /// </summary>
    public Buttons Button { get; }

    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    public byte? SpriteIndex {
        get => this._spriteIndex;
        set {
            if (this.Set(ref this._spriteIndex, value)) {
                if (this._spriteIndex == null) {
                    this._iconSet.UnsetSprite(this.Button);
                }
                else {
                    this._iconSet.SetSprite(this._spriteIndex.Value, this.Button);
                }
            }
        }
    }
}
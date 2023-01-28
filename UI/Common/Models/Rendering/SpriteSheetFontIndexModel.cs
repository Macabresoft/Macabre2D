namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A model for a single character in a <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetFontIndexModel : NotifyPropertyChanged {
    private readonly SpriteSheetFont _font;
    private byte? _spriteIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetFontIndexModel" /> class.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="character">The character.</param>
    public SpriteSheetFontIndexModel(SpriteSheetFont font, char character) {
        this._font = font;
        this.Character = character;

        if (this._font.TryGetSpriteIndex(character, out var spriteIndex)) {
            this._spriteIndex = spriteIndex;
        }
    }

    /// <summary>
    /// Gets the character.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    public byte? SpriteIndex {
        get => this._spriteIndex;
        set {
            if (this.Set(ref this._spriteIndex, value)) {
                if (this._spriteIndex == null) {
                    this._font.UnsetSprite(this.Character);
                }
                else {
                    this._font.SetSprite(this._spriteIndex.Value, this.Character);
                }
            }
        }
    }
}
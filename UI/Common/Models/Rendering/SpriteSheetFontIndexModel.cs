namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A model for a single character in a <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetFontIndexModel : PropertyChangedNotifier {
    private readonly SpriteSheetFont _font;
    private int _kerning;
    private byte? _spriteIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetFontIndexModel" /> class.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="character">The character.</param>
    public SpriteSheetFontIndexModel(SpriteSheetFont font, char character) {
        this._font = font;
        this.Character = character;

        if (this._font.TryGetSpriteCharacter(character, out var spriteCharacter)) {
            this._kerning = spriteCharacter.Kerning;
            this._spriteIndex = spriteCharacter.SpriteIndex;
        }
    }

    /// <summary>
    /// Gets the character.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets or sets the kerning.
    /// </summary>
    public int Kerning {
        get => this._kerning;
        set {
            if (this.Set(ref this._kerning, value) && this._spriteIndex.HasValue) {
                this._font.SetSprite(this._spriteIndex.Value, this.Character, this.Kerning);
            }
        }
    }

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
                    this._font.SetSprite(this._spriteIndex.Value, this.Character, this.Kerning);
                }
            }
        }
    }
}
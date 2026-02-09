namespace Macabre2D.UI.Common;

using Macabre2D.Framework;

/// <summary>
/// A model for a single character in a <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetFontIndexModel : BaseSpriteSheetIndexModel<SpriteSheetFont, char> {
    private int _kerning;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetFontIndexModel" /> class.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="character">The character.</param>
    public SpriteSheetFontIndexModel(SpriteSheetFont font, char character) : base(font, character) {
        if (this.Member.TryGetSpriteCharacter(character, out var spriteCharacter)) {
            this._kerning = spriteCharacter.Kerning;
            this.InitializeSpriteIndex(spriteCharacter.SpriteIndex);
        }
    }

    /// <summary>
    /// Gets or sets the kerning.
    /// </summary>
    public int Kerning {
        get => this._kerning;
        set {
            if (this.Set(ref this._kerning, value) && this.SpriteIndex.HasValue) {
                this.Member.SetSprite(this.SpriteIndex.Value, this.Key, this.Kerning);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnSetIndex(byte index) {
        this.Member.SetSprite(index, this.Key, this.Kerning);
    }
}
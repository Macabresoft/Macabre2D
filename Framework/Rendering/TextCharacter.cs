namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Represents a single character in a <see cref="TextWord" />.
/// </summary>
public readonly record struct TextCharacter {

    /// <summary>
    /// The sprite index.
    /// </summary>
    public readonly byte SpriteIndex;

    /// <summary>
    /// The width.
    /// </summary>
    public readonly float Width;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextCharacter" /> class.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="font">The font.</param>
    /// <param name="kerning">The additional kerning.</param>
    /// <param name="project">The game project.</param>
    public TextCharacter(SpriteSheetFontCharacter character, SpriteSheetFont font, int kerning, IGameProject project) {
        this.SpriteIndex = character.SpriteIndex;
        this.Width = font.GetCharacterWidth(character, kerning, project);
    }
}
namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a single word in a <see cref="TextLine" />.
/// </summary>
public class TextWord {
    private TextWord(IGameProject project, SpriteSheet spriteSheet, SpriteSheetFont font, int kerning, string word) {
        var fontCharacters = new List<SpriteSheetFontCharacter>();

        foreach (var character in word) {
            if (font.TryGetSpriteCharacter(character, out var fontCharacter)) {
                fontCharacters.Add(fontCharacter);
            }
        }

        var textCharacters = new List<TextCharacter>();
        foreach (var textCharacter in fontCharacters.Select(fontCharacter => new TextCharacter(fontCharacter, font, kerning, project))) {
            textCharacters.Add(textCharacter);
            this.Width += textCharacter.Width;
        }

        this.Characters = textCharacters;
        this.SpriteSheet = spriteSheet;
    }

    private TextWord(IReadOnlyCollection<TextCharacter> characters, SpriteSheet spriteSheet) {
        this.Characters = characters;
        this.SpriteSheet = spriteSheet;
        this.Width = characters.Sum(x => x.Width);
    }

    /// <summary>
    /// Gets the characters in this word.
    /// </summary>
    public IReadOnlyCollection<TextCharacter> Characters { get; }

    /// <summary>
    /// Gets the sprite sheet for this word.
    /// </summary>
    public SpriteSheet SpriteSheet { get; }

    /// <summary>
    /// Gets the width of this word, excluding spaces.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Creates a collection of <see cref="TextWord" />.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="text">The text.</param>
    /// <param name="font">The font.</param>
    /// <param name="kerning">The kerning.</param>
    /// <returns>A collection of words.</returns>
    public static IReadOnlyCollection<TextWord> CreateTextWords(
        IGameProject project,
        string text,
        SpriteSheetFont font,
        int kerning) {
        var result = new List<TextWord>();

        if (font.SpriteSheet is { } spriteSheet) {
            var splitWords = text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            result.AddRange(splitWords.Select(word => new TextWord(project, spriteSheet, font, kerning, word)));
        }

        return result;
    }
}
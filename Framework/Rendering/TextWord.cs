namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Represents a single word in a <see cref="TextLine" />.
/// </summary>
public class TextWord {
    private const char InputActionStartToken = '{';
    private const char InputActionEndToken = '}';
    
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
    public static IReadOnlyCollection<TextWord> CreateTextWords(IGameProject project, string text, SpriteSheetFont font, int kerning) {
        var result = new List<TextWord>();

        if (font.SpriteSheet is { } spriteSheet) {
            var splitWords = text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords) {
                if (word.StartsWith(InputActionStartToken) && word.EndsWith(InputActionEndToken)) {
                    var inputActionString = word[1..^1];
                    if (Enum.TryParse<InputAction>(inputActionString, out var action)) {
                        // TODO: make an input action word
                        result.Add(new TextWord(project, spriteSheet, font, kerning, action.ToString().ToUpper()));
                    }
                    else {
                        result.Add(new TextWord(project, spriteSheet, font, kerning, $"ERROR:{word}"));
                    }
                }
                else {
                    result.Add(new TextWord(project, spriteSheet, font, kerning, word));
                }
            }
        }

        return result;
    }
}
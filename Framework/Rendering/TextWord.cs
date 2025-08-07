namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Represents a single word in a <see cref="TextLine" />.
/// </summary>
public class TextWord {
    private const char InputActionEndToken = '}';
    private const char InputActionStartToken = '{';

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
        this.IsIcon = true;
        this.SpriteSheet = spriteSheet;
        this.Width = characters.Sum(x => x.Width);
    }

    /// <summary>
    /// Gets the characters in this word.
    /// </summary>
    public IReadOnlyCollection<TextCharacter> Characters { get; }

    /// <summary>
    /// Gets a value indicating whether this represents an icon.
    /// </summary>
    public bool IsIcon { get; }

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
    /// <param name="iconResolver">The icon resolver.</param>
    /// <param name="text">The text.</param>
    /// <param name="font">The font.</param>
    /// <param name="kerning">The kerning.</param>
    /// <returns>A collection of words.</returns>
    public static IReadOnlyCollection<TextWord> CreateTextWords(
        IGameProject project,
        IInputActionIconResolver iconResolver,
        string text,
        SpriteSheetFont font,
        int kerning) {
        var result = new List<TextWord>();

        if (font.SpriteSheet is { } spriteSheet) {
            var splitWords = text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords) {
                if (word.StartsWith(InputActionStartToken) && word.EndsWith(InputActionEndToken)) {
                    var inputActionString = word[1..^1];
                    if (Enum.TryParse<InputAction>(inputActionString, out var action) &&
                        iconResolver.TryGetIcon(action, InputDevice.Auto, InputActionDisplayMode.Primary, out var iconSpriteSheet, out var spriteIndex, out var iconKerning)) {
                        var width = project.UnitsPerPixel * (iconSpriteSheet.SpriteSize.X + iconKerning + kerning);
                        var textCharacter = new TextCharacter(spriteIndex.Value, width);
                        result.Add(new TextWord([textCharacter], iconSpriteSheet));
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
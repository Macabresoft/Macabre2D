namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Represents the alignment of text in a <see cref="TextLine" />
/// </summary>
public enum TextAlignment {
    Left,
    Right,
    Centered,
    Justified
}

/// <summary>
/// Represents a line of text to be rendered.
/// </summary>
public class TextLine {

    /// <summary>
    /// Gets an empty text line.
    /// </summary>
    public static readonly TextLine Empty = new(TextAlignment.Left, 0f, 0f, []);

    private readonly List<(TextWord Word, float XPosition)> _words = [];

    private TextLine(
        TextAlignment textAlignment,
        float maximumWidth,
        float spaceWidth,
        IReadOnlyCollection<TextWord> words) {
        this.Width = -spaceWidth;
        if (textAlignment == TextAlignment.Left) {
            var currentPosition = 0f;

            foreach (var word in words) {
                this._words.Add((word, currentPosition));
                var width = word.Width + spaceWidth;
                currentPosition += width;
                this.Width += width;
            }
        }
        else if (textAlignment == TextAlignment.Right) {
            var wordWidth = this.GetRequiredWidth(words, spaceWidth);
            var currentPosition = maximumWidth - wordWidth;
            foreach (var word in words) {
                this._words.Add((word, currentPosition));
                var width = word.Width + spaceWidth;
                currentPosition += width;
                this.Width += width;
            }
        }
        else if (textAlignment == TextAlignment.Centered) {
            var wordWidth = this.GetRequiredWidth(words, spaceWidth);
            var halfEmptySpace = 0.5f * (maximumWidth - wordWidth);
            var currentPosition = halfEmptySpace;
            foreach (var word in words) {
                this._words.Add((word, currentPosition));
                var width = word.Width + spaceWidth;
                currentPosition += width;
                this.Width += width;
            }
        }
        else if (textAlignment == TextAlignment.Justified) {
            var wordWidth = this.GetRequiredWidth(words, 0f);
            var emptySpace = maximumWidth - wordWidth;
            spaceWidth = words.Count > 1 ? emptySpace / (words.Count - 1) : emptySpace;
            this.Width = -spaceWidth;

            var currentPosition = 0f;

            foreach (var word in words) {
                this._words.Add((word, currentPosition));
                var width = word.Width + spaceWidth;
                currentPosition += width;
                this.Width += width;
            }
        }
        else {
            this.Width = 0f;
        }
    }

    /// <summary>
    /// Gets the width of this text line. This represents the words and spaces.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Creates a single text line from text.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="text">The text.</param>
    /// <param name="font">The font.</param>
    /// <param name="kerning">The kerning.</param>
    /// <returns>The created text line.</returns>
    public static TextLine CreateTextLine(IGameProject project, string text, SpriteSheetFont font, int kerning) {
        var spaceWidth = GetStandardSpaceCharacterWidth(project, font, kerning);
        var words = TextWord.CreateTextWords(project, text, font, kerning);
        return new TextLine(TextAlignment.Left, 0f, spaceWidth, words);
    }

    /// <summary>
    /// Creates multiple lines of text given a maximum width and a text alignment.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="text">The text.</param>
    /// <param name="maxWidth">The max width.</param>
    /// <param name="font">The font.</param>
    /// <param name="kerning">The kerning.</param>
    /// <param name="textAlignment">The text alignment.</param>
    /// <returns>The created text lines.</returns>
    public static IReadOnlyCollection<TextLine> CreateTextLines(
        IGameProject project,
        string text,
        float maxWidth,
        SpriteSheetFont font,
        int kerning,
        TextAlignment textAlignment) {
        var spaceWidth = GetStandardSpaceCharacterWidth(project, font, kerning);
        var words = TextWord.CreateTextWords(project, text, font, kerning);
        var currentLineWidth = 0f;
        var pseudoLines = new List<List<TextWord>>();
        var currentPseudoLine = new List<TextWord>();
        pseudoLines.Add(currentPseudoLine);

        foreach (var word in words) {
            currentLineWidth += word.Width;
            if (currentPseudoLine.Count == 0 || currentLineWidth < maxWidth) {
                currentPseudoLine.Add(word);
            }
            else {
                currentLineWidth = word.Width;
                currentPseudoLine = [word];
                pseudoLines.Add(currentPseudoLine);
            }

            currentLineWidth += spaceWidth;
        }

        return pseudoLines.Select(psuedoLine => new TextLine(textAlignment, maxWidth, spaceWidth, psuedoLine)).ToList();
    }

    /// <summary>
    /// Renders this text line given a start position.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="colorOverride">The color override.</param>
    /// <param name="position">The start position.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="orientation">The orientation.</param>
    public void Render(
        SpriteBatch spriteBatch,
        Color colorOverride,
        Vector2 position,
        ushort pixelsPerUnit,
        SpriteEffects orientation) {
        foreach (var (word, xPosition) in this._words) {
            var currentPosition = new Vector2(position.X + xPosition, position.Y);

            foreach (var character in word.Characters) {
                word.SpriteSheet.Draw(
                    spriteBatch,
                    pixelsPerUnit,
                    character.SpriteIndex,
                    currentPosition,
                    colorOverride,
                    orientation);

                currentPosition = new Vector2(currentPosition.X + character.Width, position.Y);
            }
        }
    }

    /// <summary>
    /// Renders this text line given a start position.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="colorOverride">The color override.</param>
    /// <param name="position">The start position.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="width">The width.</param>
    /// <param name="offset">The offset.</param>
    public void Render(
        SpriteBatch spriteBatch,
        Color colorOverride,
        Vector2 position,
        ushort pixelsPerUnit,
        SpriteEffects orientation,
        float width,
        float offset) {
        foreach (var (word, xPosition) in this._words) {
            var shouldBreak = false;
            var currentPosition = new Vector2(position.X + xPosition, position.Y);
            var characterPosition = xPosition;

            foreach (var character in word.Characters) {
                if (characterPosition + character.Width > width + offset) {
                    shouldBreak = true;
                    break;
                }

                if (characterPosition >= offset) {
                    word.SpriteSheet.Draw(
                        spriteBatch,
                        pixelsPerUnit,
                        character.SpriteIndex,
                        new Vector2(position.X + characterPosition - offset, currentPosition.Y),
                        colorOverride,
                        orientation);
                }

                characterPosition += character.Width;
                currentPosition = new Vector2(currentPosition.X + character.Width, position.Y);
            }

            if (shouldBreak) {
                break;
            }
        }
    }

    private float GetRequiredWidth(IEnumerable<TextWord> words, float spaceWidth) {
        // Start with -spaceWidth to cancel out the trailing space added by the Sum() operation.
        return -spaceWidth + words.Sum(word => word.Width + spaceWidth);
    }

    private static float GetStandardSpaceCharacterWidth(IGameProject project, SpriteSheetFont font, int kerning) {
        var result = 0f;
        if (font.TryGetSpriteCharacter(' ', out var spaceCharacter)) {
            result = font.GetCharacterWidth(spaceCharacter, kerning, project);
        }

        return result;
    }
}
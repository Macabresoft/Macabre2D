namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LegacyTextLine {
        /// <summary>
    /// Gets an empty text line.
    /// </summary>
    public static readonly LegacyTextLine Empty = new(
        new LegacyFontAsset(),
        GameProject.Empty,
        1f,
        InputActionIconResolver.Empty,
        BoundingArea.Empty,
        string.Empty);

    private readonly List<ILegacyFontLineChunk> _chunks = [];

    private Vector2 _scale = Vector2.One;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyTextLine" /> class.
    /// </summary>
    /// <param name="legacyFont">The legacy font.</param>
    /// <param name="project">The project.</param>
    /// <param name="unitsPerScreenPixel">The units per screen pixel.</param>
    /// <param name="iconResolver">The icon resolver.</param>
    /// <param name="boundingArea">The bounding area to render this within.</param>
    /// <param name="text">The text to render.</param>
    public LegacyTextLine(
        LegacyFontAsset legacyFont,
        IGameProject project,
        float unitsPerScreenPixel,
        IInputActionIconResolver iconResolver,
        BoundingArea boundingArea,
        string text) {
        if (text == string.Empty) {
            return;
        }

        var runningText = string.Empty;
        var splitWords = text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in splitWords) {
            if (word.StartsWith(TextLine.InputActionStartToken) && word.EndsWith(TextLine.InputActionEndToken)) {
                this._chunks.Add(LegacyFontLineChunk.Create(legacyFont, runningText, unitsPerScreenPixel));
                runningText = string.Empty;

                var inputActionString = word[1..^1];
                if (Enum.TryParse<InputAction>(inputActionString, out var action) &&
                    iconResolver.TryGetIcon(action, InputDevice.Auto, InputActionDisplayMode.Primary, out var iconSpriteSheet, out var spriteIndex, out var iconKerning)) {
                    var width = project.UnitsPerPixel * (iconSpriteSheet.SpriteSize.X + iconKerning);
                    var height = project.UnitsPerPixel * iconSpriteSheet.SpriteSize.Y;
                    this._chunks.Add(LegacyFontLineChunk.Create(iconSpriteSheet, spriteIndex.Value, width, height));
                }
                else {
                    this._chunks.Add(LegacyFontLineChunk.Create(legacyFont, $"ERROR:{word}", unitsPerScreenPixel));
                }
            }
            else {
                runningText += word + ' ';
            }
        }

        if (!string.IsNullOrEmpty(runningText)) {
            runningText = runningText.Remove(runningText.Length - 1);
            this._chunks.Add(LegacyFontLineChunk.Create(legacyFont, runningText, unitsPerScreenPixel));
        }

        var totalWidth = this._chunks.Sum(x => x.Width);
        var unscalableWidth = this._chunks.Where(x => x.IsIcon).Sum(x => x.Width);
        var scalableWidth = totalWidth - unscalableWidth;
        var availableWidth = boundingArea.Width - unscalableWidth;
        var scale = 1f;

        if (scalableWidth > 0f) {
            scale = availableWidth / scalableWidth;

            var unitHeight = legacyFont.CharacterPixelHeight * unitsPerScreenPixel * scale;

            if (unitHeight > boundingArea.Height) {
                scale *= boundingArea.Height / unitHeight;
            }
        }

        this._scale = new Vector2(scale);
    }

    /// <summary>
    /// Renders this text line given a start position.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="colorOverride">The color override.</param>
    /// <param name="position">The start position.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="screenPixelsPerUnit">The screen pixels per unit.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="ignoreColorForIcons">A value indicating whether to ignore color when rendering icons.</param>
    public void Render(
        SpriteBatch spriteBatch,
        Color colorOverride,
        Vector2 position,
        ushort pixelsPerUnit,
        ushort screenPixelsPerUnit,
        SpriteEffects orientation,
        bool ignoreColorForIcons) {
        var xPosition = position.X;
        
        foreach (var chunk in this._chunks) {
            var currentPosition = new Vector2(position.X + xPosition, position.Y);
            xPosition += chunk.Width;
            var color = chunk.IsIcon && ignoreColorForIcons ? Color.White : colorOverride;
            /*foreach (var character in word.Characters) {
                word.SpriteSheet.Draw(
                    spriteBatch,
                    pixelsPerUnit,
                    character.SpriteIndex,
                    currentPosition,
                    color,
                    orientation);

                currentPosition = new Vector2(currentPosition.X + character.Width, position.Y);
            }*/
        }
    }
}
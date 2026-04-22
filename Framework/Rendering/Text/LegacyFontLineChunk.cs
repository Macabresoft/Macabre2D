namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface representing a chunk of a <see cref="LegacyTextLine" />.
/// </summary>
public interface ILegacyFontLineChunk {

    /// <summary>
    /// Gets a value indicating whether this chunk represents an icon.
    /// </summary>
    bool IsIcon { get; }

    /// <summary>
    /// Gets the height of this chunk.
    /// </summary>
    float Height { get; }

    /// <summary>
    /// Gets the width of this chunk.
    /// </summary>
    float Width { get; }

    /// <summary>
    /// Resets the scale.
    /// </summary>
    /// <param name="scale">The scale.</param>
    void ResetScale(float scale);

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
        bool ignoreColorForIcons);
}

/// <summary>
/// Represents a chunk of a <see cref="LegacyTextLine"/>.
/// </summary>
public class LegacyFontLineChunk : ILegacyFontLineChunk {
    private readonly Vector2 _internalSize;
    private readonly string _text;

    private LegacyFontLineChunk(string text, Vector2 size) {
        this._text = text;
        this._internalSize = size;
        this.Width = size.X;
    }

    public bool IsIcon => false;

    /// <inheritdoc />
    public float Height { get; private set; }

    /// <inheritdoc />
    public float Width { get; private set; }

    /// <summary>
    /// Creates a <see cref="ILegacyFontLineChunk" /> from a <see cref="LegacyFontAsset" /> and some text.
    /// </summary>
    /// <param name="legacyFontAsset">The legacy font asset.</param>
    /// <param name="text">The text.</param>
    /// <param name="unitsPerScreenPixel">The number of units in a screen pixel.</param>
    /// <returns>The chunk.</returns>
    public static ILegacyFontLineChunk Create(LegacyFontAsset legacyFontAsset, string text, float unitsPerScreenPixel) {
        var size = Vector2.Zero;
        if (legacyFontAsset.Content is { } spriteFont) {
            size = spriteFont.MeasureString(text) * unitsPerScreenPixel;
        }

        return new LegacyFontLineChunk(text, size);
    }

    /// <summary>
    /// Creates a <see cref="ILegacyFontLineChunk" /> from a <see cref="SpriteSheet" /> and an index.
    /// </summary>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="width">The width.</param>
    public static ILegacyFontLineChunk Create(SpriteSheet spriteSheet, byte spriteIndex, float width, float height) => new InputActionChunk(spriteSheet, spriteIndex, width, height);

    /// <inheritdoc />
    public void ResetScale(float scale) {
        this.Width = this._internalSize.X * scale;
        this.Height = this._internalSize.Y * scale;
    }

    /// <inheritdoc />
    public void Render(
        SpriteBatch spriteBatch,
        Color colorOverride,
        Vector2 position,
        ushort pixelsPerUnit,
        ushort screenPixelsPerUnit,
        SpriteEffects orientation,
        bool ignoreColorForIcons) {
        throw new System.NotImplementedException();
    }

    private class InputActionChunk(SpriteSheet spriteSheet, byte spriteIndex, float width, float height) : ILegacyFontLineChunk {
        private readonly byte _spriteIndex = spriteIndex;
        private readonly SpriteSheet _spriteSheet = spriteSheet;

        /// <inheritdoc />
        public bool IsIcon => true;

        /// <inheritdoc />
        public float Height { get; } = height;

        /// <inheritdoc />
        public float Width { get; } = width;

        /// <inheritdoc />
        public void ResetScale(float scale) {
        }

        /// <inheritdoc />
        public void Render(
            SpriteBatch spriteBatch,
            Color colorOverride,
            Vector2 position,
            ushort pixelsPerUnit,
            ushort screenPixelsPerUnit,
            SpriteEffects orientation,
            bool ignoreColorForIcons) {
            throw new System.NotImplementedException();
        }
    }
}
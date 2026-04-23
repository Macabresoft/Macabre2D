namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LegacyTextLine {
    /// <summary>
    /// Gets an empty text line.
    /// </summary>
    public static readonly LegacyTextLine Empty = new(
        new LegacyFontAsset(),
        1f,
        BoundingArea.Empty,
        string.Empty);

    private readonly LegacyFontAsset _font;

    private readonly Vector2 _scale = Vector2.One;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyTextLine" /> class.
    /// </summary>
    /// <param name="legacyFont">The legacy font.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="unitsPerScreenPixel">The units per screen pixel.</param>
    /// <param name="text">The text to render.</param>
    public LegacyTextLine(LegacyFontAsset legacyFont, float scale, float unitsPerScreenPixel, string text) {
        this._font = legacyFont;
        this.Text = text;

        if (string.IsNullOrEmpty(text)) {
            return;
        }

        var internalSize = Vector2.Zero;
        if (legacyFont.Content is { } spriteFont) {
            internalSize = spriteFont.MeasureString(text) * unitsPerScreenPixel;
        }

        this._scale = new Vector2(scale);
        this.Width = internalSize.X * scale;
        this.Height = internalSize.Y * scale;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyTextLine" /> class.
    /// </summary>
    /// <param name="legacyFont">The legacy font.</param>
    /// <param name="unitsPerScreenPixel">The units per screen pixel.</param>
    /// <param name="boundingArea">The bounding area to render this within.</param>
    /// <param name="text">The text to render.</param>
    public LegacyTextLine(
        LegacyFontAsset legacyFont,
        float unitsPerScreenPixel,
        BoundingArea boundingArea,
        string text) {
        this._font = legacyFont;
        this.Text = text;

        if (string.IsNullOrEmpty(text)) {
            return;
        }

        var internalSize = Vector2.Zero;
        if (legacyFont.Content is { } spriteFont) {
            internalSize = spriteFont.MeasureString(text) * unitsPerScreenPixel;
        }

        var availableWidth = boundingArea.Width;
        var scale = 1f;

        if (internalSize.X > 0f && availableWidth > 0f) {
            scale = availableWidth / internalSize.X;

            var unitHeight = legacyFont.CharacterPixelHeight * unitsPerScreenPixel * scale;

            if (unitHeight > boundingArea.Height) {
                scale *= boundingArea.Height / unitHeight;
            }
        }

        this._scale = new Vector2(scale);
        this.Width = internalSize.X * scale;
        this.Height = internalSize.Y * scale;
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the width.
    /// </summary>
    public float Width { get; }

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
        if (!string.IsNullOrEmpty(this.Text)) {
            spriteBatch.Draw(
                pixelsPerUnit,
                this._font,
                this.Text,
                position,
                this._scale,
                colorOverride,
                orientation);
        }
    }
}
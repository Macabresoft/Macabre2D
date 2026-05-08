namespace Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LegacyTextLine {
    /// <summary>
    /// Gets an empty text line.
    /// </summary>
    public static readonly LegacyTextLine Empty = new(
        new LegacyFontAsset(),
        1f,
        1f,
        Vector2.Zero,
        string.Empty,
        false);

    private readonly LegacyFontAsset _font;

    private readonly Vector2 _scale = Vector2.One;
    private readonly Vector2 _offset = Vector2.Zero;

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
    /// <param name="additionalScaling">Any additional scaling to be applied.</param>
    /// <param name="size">The maximum size of this text line.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="isWidthRestricted">A value indicating whether the width is restricted.</param>
    public LegacyTextLine(
        LegacyFontAsset legacyFont,
        float unitsPerScreenPixel,
        float additionalScaling,
        Vector2 size,
        string text,
        bool isWidthRestricted) {
        this._font = legacyFont;
        this.Text = text;

        if (string.IsNullOrEmpty(text)) {
            return;
        }

        var internalSize = Vector2.Zero;
        if (legacyFont.Content is { } spriteFont) {
            internalSize = spriteFont.MeasureString(text) * unitsPerScreenPixel;
        }

        var scale = 1f;
        
        if (internalSize.Y > 0f && size.Y > 0f) {
            scale = size.Y / internalSize.Y;
        }
        
        if (additionalScaling > 0f) {
            scale *= additionalScaling;
        }

        this.Width = internalSize.X * scale;
        
        if (isWidthRestricted && this.Width > size.X && size.X > 0f) {
            scale *= size.X / this.Width;
            this.Width = internalSize.X * scale;
        }
        
        this.Height = internalSize.Y * scale;
        this._scale = new Vector2(scale);

        if (this.Height < size.Y) {
            // TODO: this only seems to work for some instances
            this._offset = new Vector2(0f, (size.Y - this.Height) * 0.5f);
        }
    }

    /// <summary>
    /// Gets the height in screen space pixels.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the width in screen space pixels.
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
                position + this._offset,
                this._scale,
                colorOverride,
                orientation);
        }
    }
}
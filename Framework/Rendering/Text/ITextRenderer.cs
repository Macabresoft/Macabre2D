namespace Macabre2D.Framework;

using Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a renderer which takes a <see cref="SpriteSheetFont" />.
/// </summary>
public interface ITextRenderer : ISpriteEntity {

    /// <summary>
    /// Gets a value indicating whether this text renderer is allowed to render a <see cref="SpriteFont" /> in a situation where it has no <see cref="SpriteSheetFont" />.
    /// </summary>
    bool AllowSpriteFont { get; }

    /// <summary>
    /// Gets or sets the font category.
    /// </summary>
    FontCategory FontCategory { get; set; }

    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    SpriteSheetFontReference FontReference { get; }

    /// <summary>
    /// Gets or sets the format of the <see cref="Text" /> or the text based on the <see cref="ResourceName" />.
    /// </summary>
    /// <remarks>
    /// Uses string.Format(...).
    /// </remarks>
    string Format { get; set; }

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    int Kerning { get; set; }

    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    string ResourceName { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Gets a value indicating whether this uses a <see cref="SpriteSheetFont" />. If not, it may be rendered as a <see cref="SpriteFont" /> in another render step.
    /// </summary>
    bool UsesSpriteSheetFont { get; }

    /// <summary>
    /// Gets the full text to be displayed by this renderer.
    /// </summary>
    /// <returns>The full text.</returns>
    string GetFullText();
}
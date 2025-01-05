namespace Macabresoft.Macabre2D.Framework;

using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Interface for a renderer which takes a <see cref="SpriteSheetFont" />.
/// </summary>
public interface ITextRenderer : IRenderableEntity, ISpriteEntity {
    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    SpriteSheetFontReference FontReference { get; }

    /// <summary>
    /// Gets or sets the font category.
    /// </summary>
    FontCategory FontCategory { get; set; }

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
    /// Gets the full text to be rendered, including any formatting.
    /// </summary>
    /// <returns>The full text.</returns>
    string GetFullText();
}
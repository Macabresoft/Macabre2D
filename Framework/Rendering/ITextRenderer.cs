namespace Macabresoft.Macabre2D.Framework;

using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a renderer which takes a <see cref="SpriteSheetFont" />.
/// </summary>
public interface ITextRenderer : IRenderableEntity {
    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    SpriteSheetFontReference FontReference { get; }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    RenderOptions RenderOptions { get; }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    Color Color { get; set; }

    /// <summary>
    /// Gets or sets the font category.
    /// </summary>
    FontCategory FontCategory { get; set; }

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
}
namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a <see cref="ITextRenderer" /> that can render a MonoGame <see cref="SpriteFont" /> under circumstances where it is required.
/// </summary>
public interface IMonoGameSpriteFontRenderer : ITextRenderer {
    /// <summary>
    /// Gets a value indicating whether this should render a MonoGame <see cref="SpriteFont" />.
    /// </summary>
    bool ShouldRenderMonoGameSpriteFont { get; }

    /// <summary>
    /// Renders this instance as a MonoGame <see cref="SpriteFont" />.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    void RenderAsMonoGameSpriteFont(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <summary>
    /// Renders this instance as a MonoGame <see cref="SpriteFont" /> with a specific color.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    /// <param name="colorOverride">The color override.</param>
    void RenderAsMonoGameSpriteFont(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);
}
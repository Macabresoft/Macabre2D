namespace Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for an entity that can render a MonoGame <see cref="SpriteFont" /> under circumstances where it is required.
/// </summary>
public interface ILegacyFontRenderer : IBaseRenderable {

    /// <summary>
    /// Called when <see cref="ShouldRenderLegacyFont" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderLegacyFontChanged;

    /// <summary>
    /// Gets a value indicating whether this should render a MonoGame <see cref="SpriteFont" />.
    /// </summary>
    bool ShouldRenderLegacyFont { get; }

    /// <summary>
    /// Renders this instance as a MonoGame <see cref="SpriteFont" />.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <summary>
    /// Renders this instance as a MonoGame <see cref="SpriteFont" /> with a specific color.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    /// <param name="colorOverride">The color override.</param>
    void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);
}
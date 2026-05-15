namespace Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for an entity that can render in screen space. For example, an entity which renders a MonoGame <see cref="SpriteFont" />.
/// </summary>
public interface IScreenSpaceRenderer : IBaseRenderable {

    /// <summary>
    /// Called when <see cref="ShouldRenderInScreenSpace" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderInScreenSpaceChanged;

    /// <summary>
    /// Gets a value indicating whether this should render in screen space.
    /// </summary>
    bool ShouldRenderInScreenSpace { get; }

    /// <summary>
    /// Renders this instance in screen space.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <summary>
    /// Renders this instance in screen space with a specific color.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    /// <param name="colorOverride">The color override.</param>
    void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);
}
namespace Macabre2D.Framework;

using System;

/// <summary>
/// An interface for a render system which exclusively renders entities that implement <see cref="IScreenSpaceRenderer" />.
/// </summary>
public interface IScreenSpaceRenderSystem {
    /// <summary>
    /// Called when <see cref="ShouldRenderInScreenSpace" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderInScreenSpaceChanged;

    /// <summary>
    /// Gets a value indicating whether this should render.
    /// </summary>
    bool ShouldRenderInScreenSpace { get; }

    /// <summary>
    /// Renders entities as defined by this system.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    void RenderInScreenSpace(FrameTime frameTime);
}
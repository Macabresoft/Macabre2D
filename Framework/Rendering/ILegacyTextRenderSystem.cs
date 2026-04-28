namespace Macabre2D.Framework;

using System;

/// <summary>
/// An interface for a render system which exclusively renders entities that implement <see cref="ILegacyTextRenderer" />.
/// </summary>
public interface ILegacyTextRenderSystem {
    /// <summary>
    /// Called when <see cref="ShouldRenderLegacyFonts" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderLegacyFontsChanged;

    /// <summary>
    /// Gets a value indicating whether this should render.
    /// </summary>
    bool ShouldRenderLegacyFonts { get; }

    /// <summary>
    /// Renders entities as defined by this system.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    void RenderLegacyFonts(FrameTime frameTime);
}
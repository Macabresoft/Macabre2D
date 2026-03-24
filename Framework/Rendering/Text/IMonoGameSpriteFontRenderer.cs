namespace Macabre2D.Framework;

using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a <see cref="ITextRenderer"/> that can render a MonoGame <see cref="SpriteFont"/> under circumstances where it is required.
/// </summary>
public interface IMonoGameSpriteFontRenderer : ITextRenderer {
    /// <summary>
    /// Gets a value indicating whether this should render a MonoGame <see cref="SpriteFont"/>.
    /// </summary>
    bool ShouldRenderMonoGameSpriteFont { get; }
}
namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Interface for a camera which tells the engine where to render any <see cref="IRenderableEntity" />.
    /// </summary>
    public interface ICamera : IEntity, IBoundable {
        /// <summary>
        /// Gets the layers to render.
        /// </summary>
        /// <value>The layers to render.</value>
        Layers LayersToRender { get; }

        /// <summary>
        /// Gets the render order.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder => 0;

        /// <summary>
        /// Gets the state of the sampler.
        /// </summary>
        /// <value>The state of the sampler.</value>
        SamplerState SamplerState { get; }

        /// <summary>
        /// Gets the view height of the camera in world units (not screen pixels).
        /// </summary>
        float ViewHeight { get; }

        /// <summary>
        /// Converts the point from screen space to world space.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The world space location of the point.</returns>
        Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point);

        /// <summary>
        /// Gets the view matrix for rendering.
        /// </summary>
        Matrix GetViewMatrix();

        /// <summary>
        /// Renders the provided entities.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="spriteBatch">The sprite batch to use while rendering.</param>
        /// <param name="entities">The entities to render.</param>
        void Render(FrameTime frameTime, SpriteBatch spriteBatch, IEnumerable<IRenderableEntity> entities);
    }
}
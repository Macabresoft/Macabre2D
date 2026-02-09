namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Extensions to <see cref="GraphicsDevice" />.
/// </summary>
public static class GraphicsDeviceExtensions {
    /// <param name="device">The graphics device.</param>
    extension(GraphicsDevice device) {
        /// <summary>
        /// Creates a render target with the desired render size.
        /// </summary>
        /// <param name="renderSize">The desired render size.</param>
        /// <returns>The render target.</returns>
        public RenderTarget2D CreateRenderTarget(Point renderSize) => new(device, renderSize.X, renderSize.Y);

        /// <summary>
        /// Creates a render target with the project's internal render resolution.
        /// </summary>
        /// <param name="project">The game project.</param>
        /// <returns>The render target.</returns>
        public RenderTarget2D CreateRenderTarget(IGameProject project) => new(device, project.InternalRenderResolution.X, project.InternalRenderResolution.Y);
    }
}